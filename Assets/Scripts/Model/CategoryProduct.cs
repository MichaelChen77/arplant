
namespace IMAV.Model
{
    public class CategoryProduct
    {
		//currently using sku to connect catagory and products
        public string sku { get; set; }
//		public string keyword { get; set; }
        public int position { get; set; }
        public long category_id { get; set; }

    }
}