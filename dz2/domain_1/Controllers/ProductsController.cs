using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using mongo.DTOs;

namespace mongo.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMongoCollection<Product> _productsCollection;

    public ProductsController(IMongoClient mongoClient)
    {
        var database = mongoClient.GetDatabase("products_db");
        _productsCollection = database.GetCollection<Product>("items");
    }

    [HttpGet]
    public async Task<IEnumerable<Product>> Get()
    {
        return await _productsCollection.Find(p => true).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetById(string id)
    {
        var product = await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (product == null)
        {
            return NotFound();
        }
        return product;
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(CreateProductDto createProductDto)
    {
        var product = new Product
        {
            Name = createProductDto.Name,
            Price = createProductDto.Price,
            InStock = createProductDto.InStock
        };
        await _productsCollection.InsertOneAsync(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateProductDto updateProductDto)
    {
        var product = await _productsCollection.Find(p => p.Id == id).FirstOrDefaultAsync();
        if (product == null)
        {
            return NotFound();
        }

        product.Name = updateProductDto.Name;
        product.Price = updateProductDto.Price;
        product.InStock = updateProductDto.InStock;

        await _productsCollection.ReplaceOneAsync(p => p.Id == id, product);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await _productsCollection.DeleteOneAsync(p => p.Id == id);
        if (result.DeletedCount == 0)
        {
            return NotFound();
        }
        return NoContent();
    }
}
