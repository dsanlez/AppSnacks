using AppLanches.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppLanches.Services
{
    public class FavoriteService
    {
        private readonly SQLiteAsyncConnection _database;
        public FavoriteService()
        {
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "favorites.db");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<FavoriteProduct>().Wait();
        }
        public async Task<FavoriteProduct> ReadAsync(int id)
        {
            try
            {
                return await _database.Table<FavoriteProduct>().Where(p => p.ProductId == id).FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<FavoriteProduct>> ReadAllAsync()
        {
            try
            {
                return await _database.Table<FavoriteProduct>().ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task CreateAsync(FavoriteProduct productFavorite)
        {
            try
            {
                await _database.InsertAsync(productFavorite);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task DeleteAsync(FavoriteProduct productFavorite)
        {
            try
            {
                await _database.DeleteAsync(productFavorite);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

