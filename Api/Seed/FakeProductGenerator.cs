﻿using Api.Model;
using Bogus;

namespace Api.Seed
{
    public static class FakeProductGenerator
    {
        public static List<Product> GenerateProductList(int count = 20)
        {
            var categories = new[] { "Категория 1", "Категория 2", "Категория 3" };
            var specialTags = new[] { "Новинка", "Популярное", "Рекомендуемое" };

            return new Faker<Product>("ru")
                .RuleFor(m => m.Id, f => f.IndexFaker + 1)
                .RuleFor(m => m.Name, f => f.Commerce.ProductName())
                .RuleFor(m => m.Description, f => f.Commerce.ProductDescription())
                .RuleFor(m => m.Category, f => f.PickRandom(categories))
                .RuleFor(m => m.SpecialTag, f => f.PickRandom(specialTags))
                .RuleFor(m => m.Price, f => Math.Round(f.Random.Double(1, 1000), 2))
                .RuleFor(m => m.Image, f => $"https://placehold.co/200")
                .Generate(count);
        }
    }
}
