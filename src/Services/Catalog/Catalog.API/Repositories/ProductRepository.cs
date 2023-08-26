using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Xml.Linq;

namespace Catalog.API.Repositories
{
    public class ProductRepository:IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            this._context = context?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products.Find(p => true).ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByName(string name)
        {
            //The Find method is a good choice for scenarios where you need to query a collection quickly and easily. However, if you need to build complex queries, the Builders filter is a better choice.

            // In general, it is best to use the simplest method that will get the job done. If the Find method is sufficient for your needs, then you should use it.However, if you need more flexibility, then you should use the Builders filter.
            var filterDefinition = Builders<Product>.Filter.Eq(p=>p.Name,name);
            return await _context.Products.Find(filterDefinition).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            var filterDefinition = Builders<Product>.Filter.Eq(p => p.Category, category);
            return await _context.Products.Find(filterDefinition).ToListAsync();
        }

        public async Task CreateProduct(Product product)
        {
             await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProduct(Product product)
        {

          var updateResult =  await _context.Products.ReplaceOneAsync(filter:g=>g.Id == product.Id, replacement:product);

          return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;

        }

        public async Task<bool> DeleteProduct(string id)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Id, id);

            var deleteResult = await _context.Products.DeleteOneAsync(filter);

            return deleteResult.IsAcknowledged && deleteResult.DeletedCount > 0;

        }
    }
}
