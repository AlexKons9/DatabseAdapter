namespace WebApplicationTestLib.Queries
{
    public static class Category
    {
        public const string GETALL = @"SELECT * 
                                       FROM [Northwind].[dbo].[Categories]";  

        public const string GET = @"SELECT * 
                                    FROM [Northwind].[dbo].[Categories]  
                                    WHERE CategoryID = @categoryId";

        public const string CREATE = @"INSERT INTO [Northwind].[dbo].[Categories] 
                                       (CategoryName, Description, Picture) 
                                       VALUES (@categoryName, @description, @picture);";

        public const string UPDATE = @"UPDATE [Northwind].[dbo].[Categories]
                                       SET CategoryName = @categoryName,
                                           Description = @description,
                                           Picture = @picture
                                       WHERE CategoryID = @categoryID";

        public const string DELETE = @"DELETE FROM [Northwind].[dbo].[Categories]
                                       WHERE CategoryID = @categoryID";
    }
}
