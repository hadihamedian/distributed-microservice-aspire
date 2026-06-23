var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("rabbitmq")
    .WithManagementPlugin();

var productDb = builder.AddPostgres("postgres-product")
    .AddDatabase("ProductDb");

var orderDb = builder.AddPostgres("postgres-order")
    .AddDatabase("OrderDb");

var productService = builder.AddProject<Projects.ProductService_API>("product-service")
    .WithReference(rabbitmq)
    .WithReference(productDb);

var orderService = builder.AddProject<Projects.OrderService_API>("order-service")
    .WithReference(rabbitmq)
    .WithReference(orderDb)
    .WithReference(productService);

var recommendationService = builder.AddProject<Projects.RecommendationService_API>("recommendation-service")
    .WithReference(productService);

builder.Build().Run();