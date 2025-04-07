using System;
using System.ComponentModel.DataAnnotations;

namespace ClipperOS.Models
{
    public class ProductModel
    {
        public Guid Id { get; set; } // Alterado de string para Guid

        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo Marca é obrigatório.")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "O campo Modelo é obrigatório.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "O campo Categoria é obrigatório.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "O campo Código de Barras é obrigatório.")]
        public string CodeBar { get; set; }

        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime Created { get; set; } // Gerado pelo banco
    }
}