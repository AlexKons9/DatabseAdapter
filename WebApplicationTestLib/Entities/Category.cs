using Newtonsoft.Json;
using System.Buffers.Text;

namespace WebApplicationTestLib.Entities
{
    public class Category
    {
        public string Id { get; set; }
        public required string CategoryName { get; set; }
        public required string Description { get; set; }
    }
}
