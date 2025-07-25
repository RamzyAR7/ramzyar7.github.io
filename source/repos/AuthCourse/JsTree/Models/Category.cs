﻿namespace JsTree.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
