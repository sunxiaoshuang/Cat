
namespace JdCat.Cat.Common.Models
{
    public class CopyProduct
    {
        /// <summary>
        /// ������id
        /// </summary>
        public int ChainId { get; set; }
        /// <summary>
        /// ��Ҫ������Ʒ���ŵ�id
        /// </summary>
        public int[] StoreIds { get; set; }
        /// <summary>
        /// ��Ҫ���Ƶ���Ʒid
        /// </summary>
        public int[] ProductIds { get; set; }
        /// <summary>
        /// ��Ҫ���Ƶ�ͼƬ����
        /// </summary>
        public string[] ImageNames { get; set; }
    }
}