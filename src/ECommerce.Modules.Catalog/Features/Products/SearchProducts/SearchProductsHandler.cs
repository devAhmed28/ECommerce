using Dapper;
using ECommerce.Modules.Catalog.Infrastructure.Database;

namespace ECommerce.Modules.Catalog.Features.Products.SearchProducts;

public sealed class SearchProductsHandler
{
    private readonly IDbConnectionFactory _connectionFactory;

	public SearchProductsHandler(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<SearchProductsResult> HandleAsync(SearchProductsQuery query)
    {
        using var connection = _connectionFactory.CreateConnection();

        var (whereClause, parameters) = BuildFilters(query);

        var countSql = $@"
            SELECT COUNT(*)
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            {whereClause};
            ";

        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var sortColumn = GetSortColumn(query.SortBy);

        var sortDirection = query.SortOrder.Equals(
            "desc", StringComparison.OrdinalIgnoreCase) ? "DESC" : "ASC";

        var offset = (query.Page - 1) * query.PageSize;

        var dataSql = $@"
            SELECT
                p.Id,
                p.Name,
                p.Description,
                p.Price,
                p.CategoryId,
                p.Status,
                p.CreatedAt,
                p.UpdatedAt,
                c.Name AS CategoryName
            FROM Products p
            INNER JOIN Categories c ON p.CategoryId = c.Id
            {whereClause}
            ORDER BY {sortColumn} {sortDirection}
            OFFSET @Offset ROWS
            FETCH NEXT @PageSize ROWS ONLY;
            ";

        parameters.Add("Offset", offset);
        parameters.Add("PageSize", query.PageSize);

        var products = await connection.QueryAsync<SearchProductsDto>(dataSql, parameters);

        var responses = products.Select(product => new SearchProductsResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CategoryId = product.CategoryId,
            CategoryName = product.CategoryName,
            Status = product.Status.ToString(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        });

        return new SearchProductsResult
        {
            Items = responses,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };
    }

    private static (
        string WhereClause,
        DynamicParameters Parameters) BuildFilters(SearchProductsQuery query)
    {
        var whereClause = "WHERE 1=1";
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            whereClause += " AND p.Name LIKE @Name";
            parameters.Add("Name", $"%{query.Name}%");
        }

        if (query.CategoryId.HasValue)
        {
            whereClause += " AND p.CategoryId = @CategoryId";
            parameters.Add(
                "CategoryId",
                query.CategoryId.Value);
        }

        if (query.MinPrice.HasValue)
        {
            whereClause += " AND p.Price >= @MinPrice";
            parameters.Add(
                "MinPrice",
                query.MinPrice.Value);
        }

        if (query.MaxPrice.HasValue)
        {
            whereClause += " AND p.Price <= @MaxPrice";
            parameters.Add(
                "MaxPrice",
                query.MaxPrice.Value);
        }

        if (query.Status.HasValue)
        {
            whereClause += " AND p.Status = @Status";
            parameters.Add(
                "Status",
                query.Status.Value);
        }

        return (whereClause, parameters);
    }

    private static string GetSortColumn(string? sortBy)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => "p.Name",
            "price" => "p.Price",
            "createdat" => "p.CreatedAt",
            "categoryname" => "c.Name",
            _ => "p.Id"
        };
    }
}