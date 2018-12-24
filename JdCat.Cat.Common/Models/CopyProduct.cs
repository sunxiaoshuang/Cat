
namespace JdCat.Cat.Common.Models
{
    public class CopyProduct
    {
        /// <summary>
        /// 连锁店id
        /// </summary>
        public int ChainId { get; set; }
        /// <summary>
        /// 需要复制商品的门店id
        /// </summary>
        public int[] StoreIds { get; set; }
        /// <summary>
        /// 需要复制的商品id
        /// </summary>
        public int[] ProductIds { get; set; }
        /// <summary>
        /// 需要复制的图片名称
        /// </summary>
        public string[] ImageNames { get; set; }
    }
}