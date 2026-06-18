using Optivio.Domin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Optivio.ServiceAbstraction.IServices.IJwtService.cs
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
