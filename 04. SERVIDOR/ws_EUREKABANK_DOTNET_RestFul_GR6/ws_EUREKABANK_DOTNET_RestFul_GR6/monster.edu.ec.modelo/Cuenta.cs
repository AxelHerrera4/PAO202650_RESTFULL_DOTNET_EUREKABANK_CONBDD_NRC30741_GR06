using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace monster.edu.ec.modelo;

[Table("cuenta")]
public class Cuenta
{
    [Key]
    [Column("numero_cuenta")]
    public string NumeroCuenta { get; set; } = string.Empty;

    [Column("saldo")]
    public decimal Saldo { get; set; }

    [Column("id_cliente")]
    public int IdCliente { get; set; }

    [Column("id_sucursal")]
    public int IdSucursal { get; set; }

    [Column("estado")]
    public string Estado { get; set; } = string.Empty;

    [ForeignKey("IdCliente")]
    public virtual Cliente? Cliente { get; set; }

    [ForeignKey("IdSucursal")]
    public virtual Sucursal? Sucursal { get; set; }
}
