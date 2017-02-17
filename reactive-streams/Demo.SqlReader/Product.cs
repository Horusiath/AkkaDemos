namespace Demo.SqlReader
{
    public sealed class Product
    {
        public int ProductID { get; }
        public string ProductName { get; }
        public int SupplierID { get; }
        public int CategoryID { get; }
        public string QuantityPerUnit { get; }
        public decimal UnitPrice { get; }
        public short UnitsInStock { get; }
        public short UnitsOnOrder { get; }
        public short ReorderLevel { get; }
        public bool Discontinued { get; }
        
        /// <summary>
        /// A POCO used by Dapper to map Northwind dbo.Products table.
        /// </summary>
        public Product(int productId, 
            string productName, 
            int supplierId, 
            int categoryId, 
            string quantityPerUnit, 
            decimal unitPrice, 
            short unitsInStock, 
            short unitsOnOrder, 
            short reorderLevel,
            bool discontinued)
        {
            this.ProductID = productId;
            this.ProductName = productName;
            this.SupplierID = supplierId;
            this.CategoryID = categoryId;
            this.QuantityPerUnit = quantityPerUnit;
            this.UnitPrice = unitPrice;
            this.UnitsInStock = unitsInStock;
            this.UnitsOnOrder = unitsOnOrder;
            this.ReorderLevel = reorderLevel;
            this.Discontinued = discontinued;
        }
    }
}