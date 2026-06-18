using Microsoft.EntityFrameworkCore;
using Optivio.Domin.Models;
using Optivio.Domin.Models.Enums;
using Optivio.Persistence.Data.DbContexts;
using System.Text.Json;


namespace Optivio.Extensions
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ObtivioDbContext>();

            // Seed Admin User
            if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                var admin = new User
                {
                    FirstName = "Admin",
                    LastName = "Optivio",
                    Email = "admin@optivio.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                    Role = UserRole.Admin,
                    Status = Status.Active,
                    Profile = new Profile
                    {
                        DateOfBirth = new DateTime(1990, 1, 1),
                        Gender = Gender.Male,
                        AvatarUrl = ""
                    }
                };
                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();
                Console.WriteLine("Admin user seeded!");
            }

            // Seed Products
            if (!await context.Products.AnyAsync())
            {
                var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "SeedData", "products.json");
                if (!File.Exists(jsonPath))
                {
                    Console.WriteLine("Seed file not found.");
                    return;
                }

                var jsonContent = await File.ReadAllTextAsync(jsonPath);
                var seedData = JsonSerializer.Deserialize<SeedDataModel>(jsonContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (seedData == null) return;

                var categories = seedData.Categories.Select(c => new ProductCategory { Name = c.Name }).ToList();
                await context.ProductCategories.AddRangeAsync(categories);
                await context.SaveChangesAsync();

                var brands = seedData.Brands.Select(b => new ProductBrand { Name = b.Name }).ToList();
                await context.ProductBrands.AddRangeAsync(brands);
                await context.SaveChangesAsync();

                foreach (var p in seedData.Products)
                {
                    var brand = await context.ProductBrands.FirstOrDefaultAsync(b => b.Name == p.BrandName);
                    var category = await context.ProductCategories.FirstOrDefaultAsync(c => c.Name == p.CategoryName);

                    if (brand == null || category == null) continue;

                    var product = new Product
                    {
                        Name = p.Name,
                        Description = p.Description,
                        Color = p.Color,
                        Gender = Enum.Parse<Gender>(p.Gender),
                        Size = p.Size,
                        LensType = Enum.Parse<LensType>(p.LensType),
                        Price = p.Price,
                        Currency = p.Currency,
                        StockQuantity = p.StockQuantity,
                        ThumbnailUrl = p.ThumbnailUrl,
                        MediaUrl = p.MediaUrl,
                        BrandId = brand.Id,
                        CategoryId = category.Id,
                        IsActive = true
                    };

                    await context.Products.AddAsync(product);
                }

                await context.SaveChangesAsync();
                Console.WriteLine("Products seeded successfully!");
            }

            await SeedFaceShapesAsync(context);
            await UpdateProductsWithRealDataAsync(context);
            await Update23ProductsAsync(context);
            await UpdateDescriptions3DAsync(context);
            await DeleteExtraProductsAsync(context);
        }

        private static async Task DeleteExtraProductsAsync(ObtivioDbContext context)
        {
            var extraProducts = await context.Products
                .Where(p => p.Id >= 49 && p.Id <= 81)
                .ToListAsync();

            if (!extraProducts.Any())
            {
                Console.WriteLine("No extra products to delete.");
                return;
            }

            context.Products.RemoveRange(extraProducts);
            await context.SaveChangesAsync();
            Console.WriteLine($"Deleted {extraProducts.Count} extra products successfully!");
        }

        private static async Task UpdateDescriptions3DAsync(ObtivioDbContext context)
        {
            if (await context.Products.AnyAsync(p => p.Id == 9 && p.Description.Contains("sharp-angled")))
            {
                Console.WriteLine("3D Descriptions already updated.");
                return;
            }

            var descriptions = new Dictionary<int, string>
    {
        { 9,  "A structured, sharp-angled frame designed for a professional and intellectual look." },
        { 10, "Handcrafted-style frames with unique detailing, perfect for those who appreciate creative and bespoke design." },
        { 11, "Striking semi-rimless frames in a deep red finish, offering a wide field of vision and a bold pop of color." },
        { 12, "Dark-tinted circular lenses set in a minimalist frame for a timeless, mysterious aesthetic." },
        { 13, "Ultra-minimalist clear frames designed for a lightweight feel and a modern, high-tech aesthetic." },
        { 14, "A classic, vintage-inspired silhouette that pays homage to traditional eyewear craftsmanship." },
        { 15, "Modern, fast-paced design with sleek lines, tailored for a sophisticated city lifestyle." },
        { 16, "Solid black rectangular frames fitted with high-quality sun lenses for maximum protection and style." },
        { 17, "Sleek, low-profile black frames that offer a mysterious and undeniably cool urban aesthetic." },
        { 18, "The quintessential black eyewear, versatile, durable, and designed to complement every face shape." },
        { 19, "Delicate black wireframes with a perfectly round shape for a vintage-inspired, artistic look." },
        { 20, "Exquisite floral-accented frames that blend high-fashion embroidery with a classic butterfly silhouette." },
        { 21, "Earthy, adventure-ready frames with a wide field of vision and a rugged yet refined build." },
        { 22, "Structured rectangular frames with a reinforced bridge, perfect for serious study and long-term comfort." },
        { 23, "Soft, neutral-toned frames that offer a subtle pop of color while maintaining a sophisticated vibe." },
        { 24, "Intellectual round frames crafted in a deep wood-tone finish for a warm, academic look." },
        { 25, "A sharp take on a legendary silhouette, featuring a deeper frame profile and a polished finish." },
    };

            var products = await context.Products
                .Where(p => descriptions.Keys.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                if (!descriptions.TryGetValue(product.Id, out var desc)) continue;
                product.Description = desc;
            }

            await context.SaveChangesAsync();
            Console.WriteLine("3D Descriptions updated successfully!");
        }

        private static async Task SeedFaceShapesAsync(ObtivioDbContext context)
        {
            if (await context.ProductFaceShapes.AnyAsync())
            {
                Console.WriteLine("FaceShapes already seeded.");
                return;
            }

            var faceShapeMap = new Dictionary<string, List<FaceShape>>
            {
                { "Classic Aviator",       new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Wayfarer Classic",      new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "Sport Shield",          new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Cat Eye Elegance",      new() { FaceShape.Round, FaceShape.Square, FaceShape.Oblong } },
                { "Round Retro",           new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "Oversized Glamour",     new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Square Pro",            new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Sport Wrap",            new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Slim Rectangular",      new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Butterfly Frame",       new() { FaceShape.Round, FaceShape.Square, FaceShape.Heart } },
                { "Titanium Rimless",      new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "Hexagonal Trendy",      new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Clubmaster Elite",      new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Reading Comfort",       new() { FaceShape.Oval, FaceShape.Round, FaceShape.Square } },
                { "Sporty Blue",           new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Vintage Round",         new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "Executive Square",      new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Gradient Sunset",       new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Blue Light Shield",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Mirror Sport",          new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Feminine Oval",         new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "Classic Tortoise",      new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "Wide Frame Bold",       new() { FaceShape.Oval, FaceShape.Round, FaceShape.Oblong } },
                { "Polarized Pro",         new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Slim Cat Eye",          new() { FaceShape.Round, FaceShape.Square, FaceShape.Oblong } },
                { "Sporty Lightweight",    new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Gold Luxury",           new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Casual Round",          new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "Tinted Fashion",        new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Geometric Edge",        new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Pilot Classic",         new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Minimalist Clear",      new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "Adventure Wrap",        new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Classic Half Rim",      new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Rose Gold Chic",        new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "Urban Square",          new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "Desert Storm",          new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "Feminine Rimless",      new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "Night Drive",           new() { FaceShape.Oval, FaceShape.Round, FaceShape.Square } },
                { "Retro Pilot",           new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "Bamboo Eco",            new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Architect",         new() { FaceShape.Oval, FaceShape.Oblong, FaceShape.Heart } },
                { "The Artisan",           new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Crimson Vista",     new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Eclipse Round",     new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Ghost",             new() { FaceShape.Oval, FaceShape.Round, FaceShape.Square } },
                { "The Heritage",          new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "The Metropolitan",      new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "The Midnight Sun",      new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Noir Vibe",         new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Onyx Essential",    new() { FaceShape.Oval, FaceShape.Round, FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Raven Circular",    new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Sakura Blossom",    new() { FaceShape.Round, FaceShape.Square, FaceShape.Heart } },
                { "The Savannah",          new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "The Scholar",           new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Taupe Trend",       new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Walnut Scholar",    new() { FaceShape.Round, FaceShape.Heart, FaceShape.Oblong } },
                { "The Wayfarer Edge",     new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "The Amber Wayfarer",    new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "The Architect Square",  new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Blush Rim",         new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Bronze Rider",      new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Classic Panto",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Cobalt Keyhole",    new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Coffee Rim",        new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Crystal Clear",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Crystal Sand",      new() { FaceShape.Oval, FaceShape.Heart, FaceShape.Oblong } },
                { "The Dual Tone Classic", new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Gold Horizon",      new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Golden Eclipse",    new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Golden Wire",       new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Havana Bold Square",new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Hexagon Shadow",    new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Honey Glow",        new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Hunter Square",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Jet Setter",        new() { FaceShape.Oval, FaceShape.Round, FaceShape.Square } },
                { "The Midnight Oval",     new() { FaceShape.Square, FaceShape.Heart, FaceShape.Oblong } },
                { "The Navy Edge",         new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Noir Halfline",     new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Obsidian Frame",    new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Olive Muse",        new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Olive Scout",       new() { FaceShape.Oval, FaceShape.Square, FaceShape.Oblong } },
                { "The Rose Bloom",        new() { FaceShape.Round, FaceShape.Square, FaceShape.Heart } },
                { "The Shadow Browline",   new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Shadow Square",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Silver Arc",        new() { FaceShape.Oval, FaceShape.Square, FaceShape.Heart } },
                { "The Skyline Frame",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Tortoise Luxe",     new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Urban Slim",        new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
                { "The Walnut Prime",      new() { FaceShape.Oval, FaceShape.Round, FaceShape.Heart } },
            };

            var products = await context.Products.ToListAsync();

            foreach (var product in products)
            {
                if (!faceShapeMap.TryGetValue(product.Name, out var shapes)) continue;

                foreach (var shape in shapes)
                {
                    context.ProductFaceShapes.Add(new ProductFaceShape
                    {
                        ProductId = product.Id,
                        FaceShape = shape
                    });
                }
            }

            await context.SaveChangesAsync();
            Console.WriteLine("FaceShapes seeded successfully!");
        }

        private static async Task UpdateProductsWithRealDataAsync(ObtivioDbContext context)
        {
            if (await context.Products.AnyAsync(p => p.ThumbnailUrl.Contains("/api/files/")))
            {
                Console.WriteLine("Products already updated with real data.");
                return;
            }

            var baseUrl = "https://backendgraduationproject1.runasp.net";

            var updates = new Dictionary<int, (string Name, string Description, string ThumbnailUrl, string MediaUrl)>
            {
                { 9,  ("The Architect",      "Minimalist rectangular frames with a clean, structured silhouette for a professional and modern look.",         $"{baseUrl}/api/files/images/The Architect.png",      $"{baseUrl}/api/files/models/The Architect.glb") },
                { 10, ("The Artisan",        "Handcrafted-inspired frames with unique detailing, blending classic craftsmanship with contemporary style.",    $"{baseUrl}/api/files/images/The Artisan.png",        $"{baseUrl}/api/files/models/The Artisan.glb") },
                { 11, ("The Crimson Vista",  "Bold crimson-accented frames that make a vibrant statement while maintaining elegant proportions.",             $"{baseUrl}/api/files/images/The Crimson Vista.png",  $"{baseUrl}/api/files/models/The Crimson Vista.glb") },
                { 12, ("The Eclipse Round",  "Perfectly circular frames with a sleek finish, inspired by celestial elegance and timeless round styling.",     $"{baseUrl}/api/files/images/The Eclipse Round.png",  $"{baseUrl}/api/files/models/The Eclipse Round.glb") },
                { 13, ("The Ghost",          "Ultra-minimalist clear frames designed for a lightweight feel and a modern, high-tech aesthetic.",              $"{baseUrl}/api/files/images/The Ghost.png",          $"{baseUrl}/api/files/models/The Ghost.glb") },
                { 14, ("The Heritage",       "Classic vintage-inspired frames with a timeless silhouette that pays homage to iconic eyewear history.",        $"{baseUrl}/api/files/images/The Heritage.png",       $"{baseUrl}/api/files/models/The Heritage.glb") },
                { 15, ("The Metropolitan",   "Urban-inspired frames designed for the city dweller, combining sharp lines with sophisticated aesthetics.",      $"{baseUrl}/api/files/images/The Metropolitan.png",   $"{baseUrl}/api/files/models/The Metropolitan.glb") },
                { 16, ("The Midnight Sun",   "Dark, sophisticated frames with a warm undertone, perfect for transitioning from day to evening wear.",         $"{baseUrl}/api/files/images/The Midnight Sun.png",   $"{baseUrl}/api/files/models/The Midnight Sun.glb") },
                { 17, ("The Noir Vibe",      "Sleek, low-profile black frames that offer a mysterious and undeniably cool urban aesthetic.",                  $"{baseUrl}/api/files/images/The Noir Vibe.png",      $"{baseUrl}/api/files/models/The Noir Vibe.glb") },
                { 18, ("The Onyx Essential", "The quintessential black eyewear, versatile, durable, and designed to complement every face shape.",            $"{baseUrl}/api/files/images/The Onyx Essential.png", $"{baseUrl}/api/files/models/The Onyx Essential.glb") },
                { 19, ("The Raven Circular", "Delicate black wireframes with a perfectly round shape for a vintage-inspired, artistic look.",                 $"{baseUrl}/api/files/images/The Raven Circular.png", $"{baseUrl}/api/files/models/The Raven Circular.glb") },
                { 20, ("The Sakura Blossom", "Exquisite floral-accented frames that blend high-fashion embroidery with a classic butterfly silhouette.",      $"{baseUrl}/api/files/images/The Sakura Blossom.png", $"{baseUrl}/api/files/models/The Sakura Blossom.glb") },
                { 21, ("The Savannah",       "Earthy, adventure-ready frames with a wide field of vision and a rugged yet refined build.",                    $"{baseUrl}/api/files/images/The Savannah.png",       $"{baseUrl}/api/files/models/The Savannah.glb") },
                { 22, ("The Scholar",        "Structured rectangular frames with a reinforced bridge, perfect for serious study and long-term comfort.",      $"{baseUrl}/api/files/images/The Scholar.png",        $"{baseUrl}/api/files/models/The Scholar.glb") },
                { 23, ("The Taupe Trend",    "Soft, neutral-toned frames that offer a subtle pop of color while maintaining a sophisticated vibe.",           $"{baseUrl}/api/files/images/The Taupe Trend.png",    $"{baseUrl}/api/files/models/The Taupe Trend.glb") },
                { 24, ("The Walnut Scholar", "Intellectual round frames crafted in a deep wood-tone finish for a warm, academic look.",                       $"{baseUrl}/api/files/images/The Walnut Scholar.png", $"{baseUrl}/api/files/models/The Walnut Scholar.glb") },
                { 25, ("The Wayfarer Edge",  "A sharp take on a legendary silhouette, featuring a deeper frame profile and a polished finish.",              $"{baseUrl}/api/files/images/The Wayfarer Edge.png",  $"{baseUrl}/api/files/models/The Wayfarer Edge.glb") },
            };

            var products = await context.Products
                .Where(p => updates.Keys.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                if (!updates.TryGetValue(product.Id, out var data)) continue;
                product.Name = data.Name;
                product.Description = data.Description;
                product.ThumbnailUrl = data.ThumbnailUrl;
                product.MediaUrl = data.MediaUrl;
            }

            await context.SaveChangesAsync();
            Console.WriteLine("Products updated with real data successfully!");
        }

        private static async Task Update23ProductsAsync(ObtivioDbContext context)
        {
            if (await context.Products.AnyAsync(p => p.Id == 26 && p.ThumbnailUrl.Contains("/api/files/")))
            {
                Console.WriteLine("23 Products already updated.");
                return;
            }

            var baseUrl = "https://backendgraduationproject1.runasp.net";

            var updates = new Dictionary<int, (string Name, string Description, string ThumbnailUrl)>
            {
                { 26, ("The Amber Wayfarer",     "Bold black frames paired with vintage yellow lenses, optimized for style and low-light clarity.",                        $"{baseUrl}/api/files/images/The Amber Wayfarer.jpg") },
                { 27, ("The Architect Square",   "Minimalist black eyewear with a flat-top design, perfect for a professional and structured look.",                      $"{baseUrl}/api/files/images/The Architect Square.jpg") },
                { 28, ("The Blush Rim",          "Subtle and sophisticated, these frames feature a soft pink hue that adds a warm, glowing touch to the face.",           $"{baseUrl}/api/files/images/The Blush Rim.jpeg") },
                { 29, ("The Bronze Rider",       "Rugged aviator-style frames with a metallic bronze finish, built for durability and a classic cool factor.",            $"{baseUrl}/api/files/images/The Bronze Rider.jpg") },
                { 30, ("The Classic Panto",      "A timeless blend of round and square shapes in a versatile mottled tortoise-shell finish.",                             $"{baseUrl}/api/files/images/The Classic Panto.jpg") },
                { 31, ("The Cobalt Keyhole",     "Classic round frames with a distinctive keyhole bridge and cool blue-gradient lenses.",                                 $"{baseUrl}/api/files/images/The Cobalt Keyhole.jpg") },
                { 32, ("The Coffee Rim",         "Sophisticated tortoise-shell accents meet warm, earthy lenses for a perfect daily accessory.",                          $"{baseUrl}/api/files/images/The Coffee Rim.jpg") },
                { 33, ("The Crystal Clear",      "Trendy transparent acetate frames that provide a clean barely-there look for any face shape.",                          $"{baseUrl}/api/files/images/The Crystal Clear.jpg") },
                { 34, ("The Crystal Sand",       "A unique semi-transparent frame with a light champagne tint, perfect for a modern, neutral aesthetic.",                 $"{baseUrl}/api/files/images/The Crystal Sand.jpg") },
                { 35, ("The Dual Tone Classic",  "Expertly crafted frames featuring a contrast between dark rims and metallic accents for a refined, professional look.", $"{baseUrl}/api/files/images/The Dual Tone Classic.jpg") },
                { 36, ("The Gold Horizon",       "A luxury-inspired rectangular frame featuring thin gold-tone temples and soft tinted lenses.",                          $"{baseUrl}/api/files/images/The Gold Horizon.jpg") },
                { 37, ("The Golden Eclipse",     "Minimalist round sunnies with a slim metal bridge and high-contrast dark lenses.",                                      $"{baseUrl}/api/files/images/The Golden Eclipse.jpg") },
                { 38, ("The Golden Wire",        "Ultra-thin, lightweight gold wireframes that provide a vintage 70s vibe with maximum comfort.",                         $"{baseUrl}/api/files/images/The Golden Wire.jpeg") },
                { 39, ("The Havana Bold Square", "Oversized, thick-rimmed square frames in a rich Havana pattern for a confident statement.",                             $"{baseUrl}/api/files/images/The Havana Bold Square.jpg") },
                { 40, ("The Hexagon Shadow",     "Modern geometric metal frames with a double bridge, offering a unique twist on the classic aviator.",                   $"{baseUrl}/api/files/images/The Hexagon Shadow.jpg") },
                { 41, ("The Honey Glow",         "Warm, amber-toned transparent frames that catch the light beautifully, offering a soft yet trendy appearance.",         $"{baseUrl}/api/files/images/The Honey Glow.jpg") },
                { 42, ("The Hunter Square",      "Rugged square frames in a dark tortoise finish, built for those who prefer a solid, sturdy aesthetic.",                 $"{baseUrl}/api/files/images/The Hunter Square.jpg") },
                { 43, ("The Jet Setter",         "A versatile, chunky black frame with rounded edges, built for all-day comfort and timeless style.",                    $"{baseUrl}/api/files/images/The Jet Setter.png") },
                { 44, ("The Midnight Oval",      "A retro-90s throwback featuring a slim, oval profile and deep-tinted lenses.",                                         $"{baseUrl}/api/files/images/The Midnight Oval.jpg") },
                { 45, ("The Navy Edge",          "Deep blue frames with sharp, clean lines, a great alternative to standard black for a modern professional.",           $"{baseUrl}/api/files/images/The Navy Edge.jpg") },
                { 46, ("The Noir Halfline",      "Classic semi-rimless clubmaster style with a bold black upper rim and a minimalist lower wire.",                        $"{baseUrl}/api/files/images/The Noir Halfline.jpg") },
                { 47, ("The Obsidian Frame",     "Sleek, jet-black rectangular frames with a high-gloss finish for a sharp and commanding presence.",                    $"{baseUrl}/api/files/images/The Obsidian Frame.jpg") },
                { 48, ("The Olive Muse",         "Earthy olive-green frames that offer a unique, artistic alternative to traditional neutral colors.",                    $"{baseUrl}/api/files/images/The Olive Muse.jpg") },
            };

            var products = await context.Products
                .Where(p => updates.Keys.Contains(p.Id))
                .ToListAsync();

            foreach (var product in products)
            {
                if (!updates.TryGetValue(product.Id, out var data)) continue;
                product.Name = data.Name;
                product.Description = data.Description;
                product.ThumbnailUrl = data.ThumbnailUrl;
                product.MediaUrl = "";
            }

            await context.SaveChangesAsync();
            Console.WriteLine("23 Products updated successfully!");
        }
    }

    public class SeedDataModel
    {
        public List<CategorySeed> Categories { get; set; } = new();
        public List<BrandSeed> Brands { get; set; } = new();
        public List<ProductSeed> Products { get; set; } = new();
    }

    public class CategorySeed { public string Name { get; set; } = null!; }
    public class BrandSeed { public string Name { get; set; } = null!; }
    public class ProductSeed
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Size { get; set; } = null!;
        public string LensType { get; set; } = null!;
        public decimal Price { get; set; }
        public string Currency { get; set; } = null!;
        public int StockQuantity { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
        public string MediaUrl { get; set; } = null!;
        public string BrandName { get; set; } = null!;
        public string CategoryName { get; set; } = null!;
    }
}