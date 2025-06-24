using Api.Data;
using Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32.SafeHandles;

namespace Api.Service
{
    public class ShoppingCartService
    {
        private readonly AppDbContext dbContext;

        public ShoppingCartService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task CreateNewCartAsync(
            string userId, int productId, int quantity)
        {
            ShoppingCart newCart = new()
            {
                UserId = userId
            };

            await dbContext.ShoppingCarts.AddAsync(newCart);
            await dbContext.SaveChangesAsync();

            CartItem newCartItem = new()
            {
                ProductId = productId,
                Quantity = quantity,
                ShoppingCartId = newCart.Id,
                Product = null
            };

            await dbContext.CartItems.AddAsync(newCartItem);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateExistingCartAsync(
            ShoppingCart shoppingCart, int productId, int newQuantity)
        {
            CartItem? cartItemInCart = shoppingCart
                .CartItems
                .FirstOrDefault(e => e.ProductId == productId);

            if (cartItemInCart == null && newQuantity > 0)
            {
                CartItem cartItem = new()
                {
                    ProductId = productId,
                    Quantity = newQuantity,
                    ShoppingCartId = shoppingCart.Id,
                    Product = null
                };

                await dbContext.CartItems.AddAsync(cartItem);
            }
            else if (cartItemInCart != null)
            {
                int updateQuantity = cartItemInCart.Quantity + newQuantity;

                if (newQuantity == 0 || updateQuantity <= 0)
                {
                    dbContext.CartItems.Remove(cartItemInCart);

                    if (shoppingCart.CartItems.Count == 1)
                    {
                        dbContext.ShoppingCarts.Remove(shoppingCart);
                    }
                }
                else
                {
                    cartItemInCart.Quantity = newQuantity;
                }
            }

            await dbContext.SaveChangesAsync();
        }

        public async Task<ShoppingCart> GetShoppingCartAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return new ShoppingCart();
            }

            ShoppingCart? shoppingCart = await dbContext
                .ShoppingCarts
                .Include(u => u.CartItems)
                .ThenInclude(u => u.Product)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (shoppingCart != null && shoppingCart.CartItems != null)
            {
                shoppingCart.TotalAmount = shoppingCart
                    .CartItems
                    .Sum(u => u.Quantity * u.Product.Price);
            }

            return shoppingCart;
        }
    }
}
