﻿namespace ITblogAPI.Infrastructure
{
    public class Pagination
    {
        private readonly int _maxItemsPerPage = 10;
        private int _itemsPerPage;

        public int Page { get; set; } = 1;
        public int ItemsPerPage
        {
            get => _itemsPerPage;
            set => _itemsPerPage = value > _maxItemsPerPage ? _maxItemsPerPage : value;
        }
    }
}
