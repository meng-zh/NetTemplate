using System.ComponentModel.DataAnnotations;


namespace Extensions
{
    /// <summary>
    /// 参数过滤例子
    /// </summary>
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public class ParameterAttributes
    {
        /// <summary>
        /// 正则匹配
        /// </summary>
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,40}$",ErrorMessage = "Characters are not allowed.")]
        public object FirstName;

        /// <summary>
        /// 枚举匹配
        /// </summary>
        [EnumDataType(typeof(ReorderLevel))]
        public object ReorderLevel { get; set; }

        /// <summary>
        /// 类型匹配
        /// DateTime/Html/Url/Password/PhoneNumber
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public object EmailAddress;

        /// <summary>
        /// 数组或字符串长度限制
        /// </summary>
        [MaxLength(255)]
        [MinLength(10)]
        public object Description { get; set; }

        /// <summary>
        /// 数值范围
        /// </summary>
        [Range(10, 1000,ErrorMessage = "Value for {0} must be between {1} and {2}.")]
        public object Weight;

        /// <summary>
        /// 时间范围
        /// </summary>
        [Range(typeof(DateTime), "1/2/2004", "3/4/2004",ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public object SellEndDate;

        /// <summary>
        /// 字符串长度
        /// </summary>
        [StringLength(4, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public object PhotoFileName;

        /// <summary>
        /// url 类型
        /// </summary>
        [Url]
        public string Domain;
    }

    [MetadataType(typeof(ProductMetadata))]
    public partial class Product
    {

    }

    public class ProductMetadata
    {
        /// <summary>
        /// 公开
        /// </summary>
        [ScaffoldColumn(true)]
        public object ProductID;

        /// <summary>
        /// 隐藏列
        /// </summary>
        [ScaffoldColumn(false)]
        public object ThumbnailPhotoFileName;

    }


#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
    public enum ReorderLevel
    {
        Zero = 0,
        Five = 5,
        Ten = 10,
        Fifteen = 15,
        Twenty = 20,
        TwentyFive = 25,
        Thirty = 30
    }
}
