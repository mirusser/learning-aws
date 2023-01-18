using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BookManager;

public class BookService
{
    private readonly IMongoCollection<Book> books;
    private readonly MongoDBConfig settings;

    public BookService(IOptions<MongoDBConfig> settings)
    {
        this.settings = settings.Value;

        MongoClient client = new(this.settings.ConnectionString);
        var database = client.GetDatabase(this.settings.DatabaseName);

        books = database.GetCollection<Book>(this.settings.BookCollectionName);
    }

    public async Task<List<Book>> GetAllAsync()
    {
        return await books
            .Find(_ => true)
            .ToListAsync();
    }

    public async Task<Book> CreateAsync(Book book)
    {
        await books.InsertOneAsync(book);

        return book;
    }
}
