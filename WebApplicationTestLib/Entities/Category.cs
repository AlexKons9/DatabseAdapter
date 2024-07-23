using System.Buffers.Text;

namespace WebApplicationTestLib.Entities
{
    public class Category
    {
        public int CategoryID { get; set; }
        public required string CategoryName { get; set; }
        public required string Description { get; set; }
        public byte[]? Picture { get; set; }
    }
}
