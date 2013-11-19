using Rozrvh.Exporters.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Exporters.Generators
{
    public class SvgGenerator
    {
        public string generateSVG(List<IExportHodina> hodiny)
        {

            string header = @"<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""800"" version=""1.0"" height=""364"">";
            string body = @"
<style type=""text/css"">
                rect.cardBack, rect.cardTop, rect.cardBottom {
                stroke-width: 0;
                outline: 1px solid black;
                fill-opacity: .5;
                stroke: #fff;
                }
                text.label {
                fill: #000;
                }
                text.periodic {
                fill: #c00;
                }
                text.odd {
                fill: #c00;
                }
                text.even {
                fill: #204a87;
                }
            </style>
  <defs/>
  <g id=""#root"">
    <g transform=""translate(7, 0)"">
      <g transform=""translate(0, 350)"" class=""info"">
        <text x=""9.0693359"" style=""font-size:12px;fill:#babdb6;"">Vytvořeno 2012-10-16</text>
        <text x=""730"" style=""font-size:12px;fill:#babdb6; text-anchor: end;"">
          <tspan id=""tspan23441"">Důležité doplňující informace si přečtěte na <tspan style=""fill:#729fcf;"">http://www.km.fjfi.cvut.cz/rozvrh/info.pdf</tspan>
          </tspan>
        </text>
      </g>
      <text y=""32"" x=""7"" style=""fill: #000; font-size: 12px; font-weight: bold; text-anchor: start; font-family: sans-serif;"">Zounar</text>
      <rect height=""290"" width=""1"" y=""37"" x=""107"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""107"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">7:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""157"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""157"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">8:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""207"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""207"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">9:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""257"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""257"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">10:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""307"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""307"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">11:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""357"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""357"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">12:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""407"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""407"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">13:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""457"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""457"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">14:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""507"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""507"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">15:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""557"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""557"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">16:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""607"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""607"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">17:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""657"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""657"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">18:30</text>
      <rect height=""290"" width=""1"" y=""37"" x=""707"" class=""timeGrid"" fill=""#ddd""/>
      <text y=""32"" x=""707"" style=""fill: #000; font-size: 12px; text-anchor: middle; font-family: sans-serif;"">19:30</text>
      <rect height=""1"" width=""725"" y=""57"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
      <rect height=""1"" width=""725"" y=""107"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
      <text y=""85"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Pondelí</text>
      <rect height=""1"" width=""725"" y=""157"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
      <text y=""135"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Úterý</text>
      <rect height=""50"" width=""100"" y=""107"" x=""507"" class=""cardBack"" fill=""#fff""/>
      <rect height=""33.3333333333"" width=""100"" y=""107"" x=""507"" class=""cardTop"" fill=""#c17d11""/>
      <rect height=""16.6666666667"" width=""100"" y=""140.333333333"" x=""507"" class=""cardBottom"" fill=""#fff""/>
      <rect stroke-opacity=""0.5"" fill-opacity=""0"" height=""48"" width=""98"" stroke=""#c17d11"" y=""108"" x=""508"" stroke-width=""2"" class=""cardFrame""/>
      <text y=""129"" x=""557"" style=""stroke-width: 2px; stroke: #fff;             stroke-linejoin: miter; dominant-baseline: middle;             font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans;"">EKO1</text>
      <text y=""129"" x=""557"" style=""dominant-baseline: middle;             font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans;"" class=""label&#10;                "">EKO1</text>
      <text y=""152"" x=""602"" style=""fill: #000; dominant-baseline: middle; font-size:             9px; text-anchor: end; font-family: sans;"" class=""classroomText"">T-201</text>
      <text y=""152"" x=""512"" style=""fill: #000; dominant-baseline: middle; font-size:             9px; text-anchor: start; font-family: sans;"" class=""lecturerText"">Zounar</text>
      <rect height=""50"" width=""100"" y=""107"" x=""607"" class=""cardBack"" fill=""#fff""/>
      <rect height=""33.3333333333"" width=""100"" y=""107"" x=""607"" class=""cardTop"" fill=""#c17d11""/>
      <rect height=""16.6666666667"" width=""100"" y=""140.333333333"" x=""607"" class=""cardBottom"" fill=""#fff""/>
      <rect stroke-opacity=""0.5"" fill-opacity=""0"" height=""48"" width=""98"" stroke=""#c17d11"" y=""108"" x=""608"" stroke-width=""2"" class=""cardFrame""/>
      <text y=""129"" x=""657"" style=""stroke-width: 2px; stroke: #fff;             stroke-linejoin: miter; dominant-baseline: middle;             font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans;"">EKO1cv</text>
      <text y=""129"" x=""657"" style=""dominant-baseline: middle;             font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans;"" class=""label&#10;                "">EKO1cv</text>
      <text y=""152"" x=""702"" style=""fill: #000; dominant-baseline: middle; font-size:             9px; text-anchor: end; font-family: sans;"" class=""classroomText"">T-201</text>
      <text y=""152"" x=""612"" style=""fill: #000; dominant-baseline: middle; font-size:             9px; text-anchor: start; font-family: sans;"" class=""lecturerText"">Zounar</text>
      <rect height=""1"" width=""725"" y=""207"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
      <text y=""185"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Streda</text>
      <rect height=""1"" width=""725"" y=""257"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
      <text y=""235"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Ctvrtek</text>
      <rect height=""1"" width=""725"" y=""307"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
      <text y=""285"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Pátek</text>
    </g>
  </g>
            ";
            string tail = "</svg>";
            return header + body + tail;
        }

        public string RenderHodina()
        {
            return "";
        }
    }
}