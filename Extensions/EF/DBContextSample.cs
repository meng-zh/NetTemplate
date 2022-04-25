using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Extensions.EF
{
    /// <summary>
    /// DBContext例子
    /// </summary>
    public class DBContextSample : DbContext
    {
        public DbSet<TBModel> TBModel { get; set; }

        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        /// <summary>
        /// 属性包实体类型 
        /// 目前仅支持将 Dictionary<string, object> 作为属性包实体类型
        /// 必须配置为具有唯一名称的共享类型实体 类型，并且必须使用 Set 调用实现相应的 DbSet 属性
        /// 这些实体类型没有阴影属性，EF 会改为创建索引器属性
        /// </summary>
        public DbSet<Dictionary<string, object>> Blogs2 => Set<Dictionary<string, object>>("Blog2");

        public DBContextSample(DbContextOptions<DBContextSample> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=Test");
            /*
             * SqlServer   .UseSqlServer(connectionString)  Microsoft.EntityFrameworkCore.SqlServer
             *              connectionString = "Server=192.168.0.1;;Database=Test;User Id=su;Password=123456;"
             * Mysql       .UseMySql(connectionString)      Pomelo.EntityFrameworkCore.MySql
             *              connectionString = "server=192.168.0.1;user=root;password=123456;database=Test"
             * SQLite      .UseSqlite(connectionString)     Microsoft.EntityFrameworkCore.Sqlite
             *              connectionString = "Data Source=c:\mydb.db;Version=3;Password=123456;"
             */
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //属性包实体类型  配置
            modelBuilder.SharedTypeEntity<Dictionary<string, object>>(
            "Blog2", bb =>
            {
                bb.Property<int>("BlogId");
                bb.Property<string>("Url");
                bb.Property<DateTime>("LastUpdated");
            });
       
            modelBuilder.Entity<TBModel>(e =>
            {
                // e.Property(x => x.Id).ValueGeneratedOnAdd();
                //.HasDefaultValue(3); 默认值
                //.HasDefaultValueSql("getdate()");创建时间戳
                //..

                //使用显式值替代值生成，只需将属性设置为该属性类型的 CLR 默认值（string 为 null，int 为 0，Guid 为 Guid.Empty，等等）以外的任意值
                e.Property(b => b.LastUpdated2).ValueGeneratedOnAddOrUpdate()
                .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Save);
            });
            //在数据库中计算
            //(虚拟计算列，每次从数据库中提取时会计算其值）
            //modelBuilder.Entity<Person>()
            //    .Property(p => p.DisplayName)
            //    .HasComputedColumnSql("[LastName] + ', ' + [FirstName]");
            //stored: true 持久化计算列
            //modelBuilder.Entity<Person>()
            //.Property(p => p.NameLength)
            //.HasComputedColumnSql("LEN([LastName]) + LEN([FirstName])", stored: true);

            
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Blog)//一行对多行
                .WithMany(b => b.Posts)
                .HasForeignKey(p => p.BlogUrl)
                .HasPrincipalKey(b => b.Url);
            ///blog中创建名为 LastUpdated的阴影属性
            ///获取和更改阴影属性
            ///context.Entry(myBlog).Property("LastUpdated").CurrentValue = DateTime.Now;
            ///var blogs = context.Blogs.OrderBy(b => EF.Property<DateTime>(b, "LastUpdated"));
            modelBuilder.Entity<Blog>().Property<DateTime>("LastUpdated");
            ///配置索引器
            modelBuilder.Entity<Blog>().IndexerProperty<DateTime>("LastUpdated");

            //检查约束  任何违反约束的插入或修改数据的尝试都将失败
            //modelBuilder.Entity<Product>().HasCheckConstraint("CK_Prices", "[Price] > [DiscountedPrice]", c => c.HasName("CK_Product_Prices"));

            // .HasAlternateKey(c => c.LicensePlate);//备用键

            ///枚举值转换
            modelBuilder
            .Entity<TBModel>()
            .Property(e => e.Mount)
            .HasConversion(
            v => v.ToString(),
            v => (EquineBeast)Enum.Parse(typeof(EquineBeast), v));

            base.OnModelCreating(modelBuilder);
        }
        /// <summary>
        /// 批量配置值转换器
        /// 更多查看 https://docs.microsoft.com/zh-cn/ef/core/modeling/value-conversions
        /// </summary>
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<Currency>()
                .HaveConversion<CurrencyConverter>();
        }
        /// <summary>
        /// 初始化创建表
        /// </summary>
        /// <param name="services"></param>
        public static void EnsureCreated(IServiceCollection services)
        {
            using (var scope = services.BuildServiceProvider().CreateScope())
            {
                using (var db = scope.ServiceProvider.GetRequiredService<DBContextSample>())
                {
                    db.Database.EnsureCreated();
                }
            }
        }
    }
    public class Currency
    {
        public decimal Amount   { get; set; }
        public Currency(decimal v)
        {
            Amount = v;
        }
    }
    /// <summary>
    /// 类型转换
    /// </summary>
    public class CurrencyConverter : ValueConverter<Currency, decimal>
    {
        public CurrencyConverter()
            : base(
                v => v.Amount,
                v => new Currency(v))
        {
        }
    }
    [Table("TBModel")]
    [Comment("This is an example table model!")]
    ///配置符合索引  .HasIndex(p => new { p.FirstName, p.LastName });
    ///IsUnique = true 索引唯一性  .IsUnique();
    ///配置索引名  .HasDatabaseName("Index_Name");
    ///指定筛选索引或部分索引  .HasFilter("[LastName] IS NOT NULL");
    ///索引包含列  .IncludeProperties(p => new { p.Title, p.PublishedOn});
    [Index(nameof(FirstName), nameof(LastName), IsUnique = true, Name = "Index_Name")]
    public class TBModel
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        public TBModel()

        {
        }
        private DBContextSample Context { get; set; }
        /// <summary>
        /// 注入的 DbContext 可用于选择性地访问数据库，以获取相关实体的信息
        /// </summary>
        /// <param name="context"></param>
        private TBModel(DBContextSample context)
        {
            Context = context;
        }

#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑声明为可以为 null。
        /// <summary>
        /// 设置主键
        ///  .HasKey(c => c.Id);
        ///  组合键只能使用 Fluent API 进行配置
        ///  .HasKey(c => new { c.Id, c.FirstName });
        /// </summary>
        [Key]
        ///.ValueGeneratedOnAdd();
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// 更新或添加时生成起值
        ///   .ValueGeneratedOnAddOrUpdate();
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime LastUpdated2 { get; set; }

        /// <summary>
        /// 图片二进制保存 类型MediumBlob
        /// </summary>
        [Column(TypeName = "MediumBlob")]
        public byte[] ProfileImage { get; set; }

        /// <summary>
        /// 列名定义 类型定义
        /// .HasColumnType("decimal(5, 2)");
        /// </summary>
        [Column("TB_Rating",TypeName = "decimal(5, 2)")]
        public decimal Rating { get; set; }

        /// <summary>
        /// 必须
        /// </summary>
        [Required]
        ///并发标志  .IsConcurrencyToken();
        ///乐观并发控制
        [ConcurrencyCheck]
        public string FirstName { get; set; }

        /// <summary>
        /// 长度限制
        /// .HasMaxLength(500)
        /// </summary>
        [MaxLength(500)]
        public string Url { get; set; }

        /// <summary>
        /// 精度限制  e.Property(b => b.Score).HasPrecision(14, 2);
        /// </summary>
        [Precision(14, 2)]
        public decimal Score { get; set; }

        /// <summary>
        /// 定义表示秒的小数部分所需的最大位数
        ///   .HasPrecision(3);
        /// </summary>
        [Precision(3)]
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// 不映射到表
        /// </summary>
        [NotMapped]
        public DateTime LoadedFromDatabase { get; set; }

        /// <summary>
        /// 表示 Unicode 和非 Unicode 文本数据
        /// 例如，在 SQL Server 中，nvarchar(x)用于表示 UTF-16 中的 Unicode 数据，而varchar(x)用于表示非 Unicode 数据
        /// (不支持此概念的数据库无效）
        /// </summary>
        [Unicode(false)]
        [MaxLength(22)]
        public string Isbn { get; set; }

        /// <summary>
        /// 列顺序
        /// </summary>
        [Column(Order = 2)]
        public string LastName { get; set; }

        /// <summary>
        /// Timestamp/rowversion 是每次插入行或更新行时数据库自动生成新值的属性。 该属性也被视为并发令牌，确保在查询后要更新的行发生更改时得到异常。
        /// 数据库中的 ROWVERSION 列
        ///  .IsRowVersion();
        /// </summary>
        [Timestamp]
        public byte[] Timestamp { get; set; }

        private string _validatedUrl;
        /// <summary>
        /// 配置支持字段
        /// .HasField("_validatedUrl");
        /// </summary>
        [BackingField(nameof(_validatedUrl))]
        public string Url2
        {
            get { return _validatedUrl; }
        }

        public EquineBeast Mount { get; set; }

    }
    /// <summary>
    /// 列表将导致 BlogId 阴影属性引入 Post 实体
    /// </summary>
    /// 配置索引  .HasIndex(b => b.Url);
    [Index(nameof(Url))]
    public class Blog
    {
        public int BlogId { get; set; }
        public string Url { get; set; }

        public List<Post> Posts { get; set; }

        private readonly Dictionary<string, object> _data = new Dictionary<string, object>();

        public object this[string key]
        {
            get => _data[key];
            set => _data[key] = value;
        }
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        public string BlogUrl { get; set; }
        public Blog Blog { get; set; }
    }

    public enum EquineBeast
    {
        Donkey,
        Mule,
        Horse,
        Unicorn
    }
}
