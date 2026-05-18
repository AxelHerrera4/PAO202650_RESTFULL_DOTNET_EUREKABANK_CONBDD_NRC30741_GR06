using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace monster.edu.ec.modelo;

[Table("sucursal")]
public class Sucursal
{
    [Key]
    [Column("id_sucursal")]
    public int IdSucursal { get; set; }

    [Column("nombre")]
    public string Nombre { get; set; } = string.Empty;

    [Column("ciudad")]
    public string Ciudad { get; set; } = string.Empty;
}
