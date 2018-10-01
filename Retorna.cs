using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GridView
{
    public static class Retorna
    {
        public static String Grid(Grid Grid)
        {
            var Campos = Metodos.RetornaCabecalho(Grid.Registros.FirstOrDefault());            

            var html = Grid.Load ? "" : $"<div class='row' id='componentes{Grid.Nome}' style='margin-bottom:10px;'><div class='col-md-8 col-12'><div class='d-flex'><input class='form-control form-control-sm' id='{"txtpesquisar" + Grid.Nome}' type='text' style='margin-right:5px;' /><button type='button' class='btn btn-sm btn-primary' id='{"btnpesquisar" + Grid.Nome}' style='margin-right:5px;'><i class='fa fa-search' aria-hidden='true'></i></button><select class='form-control form-control-sm' id='{"selectrange" + Grid.Nome}' style='width:70px;'><option value='0'>10</option><option value='1'>20</option><option value='2'>40</option><option value='3'>80</option></select></div></div></div>";

            html += $"<div id='tabgrid{Grid.Nome}'><table class='table table-sm table-bordered py-3'><thead><tr>";

            for (int i = 0; i < Campos.Length; i++)
            {
                html += Campos[i];
            }

            html += "</tr></thead><tbody>";

            var Registros = Grid.Registros;

            var total = Registros.Count();

            var range = Grid.Range.Equals(Grid.range.Dez) ? 10 : Grid.Range.Equals(Grid.range.Vinte) ? 20 : Grid.Range.Equals(Grid.range.Quarenta) ? 40 : 80;

            var totaldepaginas = Convert.ToInt32(Math.Ceiling((float)total / range));

            var pagina = Grid.Pagina;

            pagina = pagina < 0 ? 0 : pagina;

            pagina = pagina > totaldepaginas - 1 ? totaldepaginas - 1 : pagina;

            var paginareal = pagina + 1;

            pagina = pagina * range;

            range = pagina + range > total ? total - pagina : range;

            Registros = Registros.Skip(pagina).Take(range);//.ToList().GetRange(pagina, range);

            foreach (var Registro in Registros)
            {
                html += "<tr>";

                var infos = Metodos.RetornaCorpo(Registro);

                for (int i = 0; i < infos.Length; i++)
                {
                    html += infos[i];
                }

                html += "</tr>";
            }

            html += $"</tbody><tfoot><tr><td colspan='1000'><div class='card-header d-flex justify-content-between'><span>Exibindo {Registros.Count()} de um total de {total} Registro(s); {totaldepaginas} Paginas.</span> <div class='d-flex' style='width:200px;'><button class='btn btn-sm' id='btnprimeiro{Grid.Nome}'><i class='fa fa-angle-double-left' aria-hidden='true'></i></button><button class='btn btn-sm' id='btnanterior{Grid.Nome}'><i class='fa fa-angle-left' aria-hidden='true'></i></button><input class='form-control form-control-sm' id='pagina{Grid.Nome}' ultima='{totaldepaginas}' value='{paginareal}' type='number' style='width:80px;text-align:center;'/><button class='btn btn-sm' id='btnproximo{Grid.Nome}'><i class='fa fa-angle-right' aria-hidden='true'></i></button><button class='btn btn-sm' id='btnultimo{Grid.Nome}'><i class='fa fa-angle-double-right' aria-hidden='true'></i></button></div></div></tr></tfoot></table></div>";

            var scripts = "<script> function AtualizaGrid"+Grid.Nome+ "() { $('#tabgrid" + Grid.Nome + "').load('/Home/AtualizaGrid/', { pesquisa: $('#txtpesquisar" + Grid.Nome + "').val(), pagina: parseInt($('#pagina" + Grid.Nome + "').val() - 1), range: $('#selectrange" + Grid.Nome + "').val() }, function (r, s, x) { if (s == 'error') { alert('erro'); } }); } $('#btnpesquisar" + Grid.Nome + "').click(function (e) { e.defaultPrevented = true; AtualizaGrid" + Grid.Nome + "(); }); $('#selectrange" + Grid.Nome + "').change(AtualizaGrid" + Grid.Nome + "); $('#tabgrid" + Grid.Nome + "').on('change', '#pagina" + Grid.Nome + "', function () { if (parseInt($(this).val() - 1) > 0) { AtualizaGrid" + Grid.Nome + "(); } else {  $(this).val(1); } }); $('#tabgrid" + Grid.Nome + "').on('click', '#btnprimeiro" + Grid.Nome + "', function () { $('#tabgrid" + Grid.Nome + "').load('/Home/AtualizaGrid/', { pesquisa: $('#txtpesquisar" + Grid.Nome + "').val(), pagina: 0, range: $('#selectrange" + Grid.Nome + "').val() }, function (r, s, x) { if (s == 'error') { alert('erro'); } }); }); $('#tabgrid" + Grid.Nome + "').on('click', '#btnanterior" + Grid.Nome + "', function () { $('#tabgrid" + Grid.Nome + "').load('/Home/AtualizaGrid/', { pesquisa: $('#txtpesquisar" + Grid.Nome + "').val(), pagina: parseInt($('#pagina" + Grid.Nome + "').val() - 2), range: $('#selectrange" + Grid.Nome + "').val() }, function (r, s, x) { if (s == 'error') { alert('erro'); } }); }); $('#tabgrid" + Grid.Nome + "').on('click', '#btnproximo" + Grid.Nome + "', function () { $('#tabgrid" + Grid.Nome + "').load('/Home/AtualizaGrid/', { pesquisa: $('#txtpesquisar" + Grid.Nome + "').val(), pagina: parseInt($('#pagina" + Grid.Nome + "').val()), range: $('#selectrange" + Grid.Nome + "').val() }, function (r, s, x) { if (s == 'error') { alert('erro'); } }); }); $('#tabgrid" + Grid.Nome + "').on('click', '#btnultimo" + Grid.Nome + "', function () { $('#tabgrid" + Grid.Nome + "').load('/Home/AtualizaGrid/', { pesquisa: $('#txtpesquisar" + Grid.Nome + "').val(), pagina: parseInt($('#pagina" + Grid.Nome + "').attr('ultima')), range: $('#selectrange" + Grid.Nome + "').val() }, function (r, s, x) { if (s == 'error') { alert('erro'); } }); });</script>";
            return Grid.Load ? html : html + scripts;
        }
    }

    public class Metodos
    {        
        public static string[] RetornaCabecalho(dynamic obj)
        {
            var props = (System.Reflection.PropertyInfo[])obj.GetType().GetProperties();
            
            return props.Select(x => $"<th>{x.Name}</th>" ).ToArray();
        }

        public static string[] RetornaCorpo(dynamic obj)
        {
            var props = (System.Reflection.PropertyInfo[])obj.GetType().GetProperties();

            return props.Select(x => $"<td>{x.GetValue(obj, null)}</td>").ToArray();
        }
    }

    public class Grid
    {
        public Grid()
        {
            this.Load = HttpContext.Current.Request.Headers.AllKeys.Contains("X-Requested-With"); //var tt = r.AllKeys.Contains("X-Requested-With");
        }

        public IEnumerable<dynamic> Registros { get; set; }

        public int Pagina { get; set; }

        public range Range { get; set; }

        public bool Load { get; }

        public string urlRetorno { get; set; }

        public string Nome { get; set; }

        public enum range
        {
            Dez, Vinte, Quarenta, Oitenta
        }
    }
}
