﻿namespace MyJour.Models
{
    public class Student : User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int ClassId { get; set; }
        //int HealthId { get; set; }  Пока нет, скоро поменяется
    }
}
