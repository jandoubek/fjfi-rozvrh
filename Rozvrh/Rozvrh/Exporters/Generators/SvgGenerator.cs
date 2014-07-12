using Rozvrh.Controllers;
using Rozvrh.Exporters.Common;
using Rozvrh.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace Rozvrh.Exporters.Generators
{
    /// <summary>
    ///  Class generating string in SVG format from list of lectures.
    /// </summary>
    public class SvgGenerator
    {
        private Dictionary<DayOfWeek, int> doWMapping;

        public SvgGenerator()
        {
            doWMapping = new Dictionary<DayOfWeek, int>()
            {
                {DayOfWeek.Monday,57},
                {DayOfWeek.Tuesday,107},
                {DayOfWeek.Wednesday,157},
                {DayOfWeek.Thursday,207},
                {DayOfWeek.Friday,257}
            };

        }

        /// <summary> 
        /// Generates string in SVG format.
        /// </summary> 
        /// <returns> String following SVG XML format. </returns>
        /// <param name="lectures">List of lectures with ExportLecture interface to export.</param>
        /// <param name="title">Title rendered at the top of the SVG.</param>
        public string generateSVG(List<TimetableField> ttFields, string title, DateTime created, string linkToInformation)
        {
            List<ExportLecture> lectures = new List<ExportLecture>();
            foreach (var ttf in ttFields)
            {
                lectures.Add(new ExportLecture(ttf));
            }
            string header = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<svg xmlns=""http://www.w3.org/2000/svg"" xmlns:xlink=""http://www.w3.org/1999/xlink"" width=""800"" version=""1.0"" height=""364"">";

            string tail = @"    </g>
  </g>
</svg>";

            return header + headerStyles() + headerRootAndBottoms(created, linkToInformation) + headerTitle(title)
                + headerHours() + headerDays() + RenderLectures(lectures) + tail;
        }


        private string RenderLectures(List<ExportLecture> lectures)
        {
            string res = "";
            if (lectures != null)
            {
                lectures.Sort((x, y) => x.Day == y.Day ?
                DateTime.Compare(x.StartTime, y.StartTime) :
                x.Day.CompareTo(y.Day));
                List < List < ExportLecture >> groups = new LecturesGroupDivider().divideToGroups(lectures);
                foreach (var group in groups)
                {
                    foreach (var lecture in group)
                    {
                        //Dont render fake lectures created in grouping process
                        if (lecture.Name != null)
                        {
                            res += RenderLecture(lecture, group.Count, group.IndexOf(lecture));
                        }

                    }
                }
            }

            return res;
        }


        private string RenderLecture(ExportLecture lecture, int split, int pos)
        {
            string res = "";

            double height = 50.0 / split;
            double width = xSpan(lecture.Length);
            double x = xStartHour(lecture.StartTime);
            double y = yStartDay(lecture.Day);
            double dy = height * pos;

            //Borders
            res += String.Format(new NumberFormatInfo(), "<rect height=\"{0}\" width=\"{1}\" y=\"{2}\" x=\"{3}\" class=\"cardBack\" fill=\"#fff\"/>",
               height, width, y + dy, x) + System.Environment.NewLine;
            //x="{$x + 1}" y="{$y + $dy + 1}" width="{$width - 2}" height="{$height - 2}"
            res += String.Format(new NumberFormatInfo(), "<rect stroke-opacity=\"0.5\" fill-opacity=\"0\" height=\"{0}\" width=\"{1}\" stroke=\"{4}\" y=\"{2}\" x=\"{3}\" stroke-width=\"2\" class=\"cardFrame\"/>",
                height - 2, width - 2, y + dy + 1, x + 1, lecture.DepartementColor) + System.Environment.NewLine;


            if (split >= 3)
            {
                //Fills
                res += String.Format(new NumberFormatInfo(), "<rect height=\"{0}\" width=\"{1}\" y=\"{2}\" x=\"{3}\" class=\"cardTop\" fill=\"{4}\"/>",
                    height / (4 - split), width, y + dy, x, lecture.DepartementColor) + System.Environment.NewLine;
                //Texts
                //x="{$x + $width div 2}" y="{$y + $dy + $height div 5 * 2 + 6}"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"stroke-width: 2px; stroke: #fff; stroke-linejoin: miter; font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans-serif;\">{2}</text>",
                    y + dy + height / 5 * 2 + 6, x + width / 2, lecture.Name) + System.Environment.NewLine;
                //x="{$x + $width div 2}" y="{$y + $dy + $height div 5 * 2 + 6 }"
                //if red in krbalek then it have class=\"label&#10; periodic\"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans-serif;\" class=\"label&#10; {3}\">{2}</text>",
                    y + dy + height / 5 * 2 + 6, x + width / 2, lecture.Name, lecture.RegularLecture ? "" : "periodic") + System.Environment.NewLine;
                //Room
                //x="{$x + $width - 5}" y="{$y + $dy + $height - 3}"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"fill: #000; font-size: 9px; text-anchor: end; font-family: sans-serif;\" class=\"classroomText\">{2}</text>",
                    y + dy + height - 5, x + width - 5, lecture.Room) + System.Environment.NewLine;
                //Lecturer
                //x="{$x + 5}" y="{$y + $dy + $height - 3}"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"fill: #000; font-size:9px; text-anchor: start; font-family: sans-serif;\" class=\"lecturerText\">{2}</text>",
                    y + dy + height - 5, x + 5, lecture.Lecturer) + System.Environment.NewLine;
            }
            else
            {
                //Fills
                //x="{$x}" y="{$y + $dy}" width="{$width}" height="{$height - $height div (4 - $split)}"
                res += String.Format(new NumberFormatInfo(), "<rect height=\"{0}\" width=\"{1}\" y=\"{2}\" x=\"{3}\" class=\"cardTop\" fill=\"{4}\"/>",
                    height - (height / (4 - split)), width, y + dy, x, lecture.DepartementColor) + System.Environment.NewLine;
                //x="{$x}" y="{$y + $dy + $height - $height * 1 div (4 - $split)}" width="{$width}" height="{$height div (4 - $split)}"
                res += String.Format(new NumberFormatInfo(), "<rect height=\"{0}\" width=\"{1}\" y=\"{2}\" x=\"{3}\" class=\"cardBottom\" fill=\"#fff\"/>",
                    height / (4 - split), width, y + dy + (height - (height / (4 - split))), x) + System.Environment.NewLine;
                //Texts
                //x="{$x + $width div 2}" y="{$y + $dy + $height div 5 * 2 + 4 - $split * 2}"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"stroke-width: 2px; stroke: #fff; stroke-linejoin: miter;  font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans-serif;\">{2}</text>",
                    y + dy + height / 5 * 2 + 4 - split * 2, x + width / 2, lecture.Name) + System.Environment.NewLine;
                //x="{$x + $width div 2}" y="{$y + $dy + $height div 5 * 2 + 4 - $split * 2}"
                //if red in krbalek then it have class=\"label&#10; periodic\"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"font-size: 12px; font-weight: bold; text-anchor: middle; font-family: sans-serif;\" class=\"label&#10; {3}\">{2}</text>",
                    y + dy + height / 5 * 2 + 4 - split * 2, x + width / 2, lecture.Name, lecture.RegularLecture ? "" : "periodic") + System.Environment.NewLine;
                //Room
                //x="{$x + $width - 5}" y="{$y + $dy + $height - 6 + $split}"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"fill: #000; font-size: 9px; text-anchor: end; font-family: sans-serif;\" class=\"classroomText\">{2}</text>",
                    y + dy + height - 6 + split, x + width - 5, lecture.Room) + System.Environment.NewLine;

                //Lecturer
                //x="{$x + 5}" y="{$y + $dy + $height - 6 + $split}"
                res += String.Format(new NumberFormatInfo(), "<text y=\"{0}\" x=\"{1}\" style=\"fill: #000; font-size:9px; text-anchor: start; font-family: sans-serif;\" class=\"lecturerText\">{2}</text>",
                    y + dy + height - 6 + split, x + 5, lecture.Lecturer) + System.Environment.NewLine;
            }




            return res;
        }

        private int xStartHour(DateTime startTime)
        {
            float time = startTime.Hour * 60 + startTime.Minute;
            float seventhirty = 7 * 60 + 30;
            return (int)Math.Floor(107 + (time - seventhirty) * 50 / 60);
        }

        private int xSpan(TimeSpan length)
        {
            return (int)Math.Floor(length.TotalMinutes * 50 / 60);
        }

        private int yStartDay(DayOfWeek d)
        {
            return doWMapping[d];
        }

        private int yEndDay(DayOfWeek d)
        {
            return yStartDay(d) + 50;
        }

        private string headerStyles()
        {
            return @"
  <style type=""text/css"">
                rect.cardBack, rect.cardTop, rect.cardBottom {
                stroke-width: 0;
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
";
        }

        private string headerRootAndBottoms(DateTime created, string link)
        {
            return @"
 <g id=""#root"">
    <g transform=""translate(7, 0)"">
      <g transform=""translate(0, 350)"" class=""info"">
        <!-- CREATED TEXT -->
        <text x=""9.0693359"" style=""font-size:12px;fill:#babdb6;"">Vytvořeno " + String.Format("{0}-{1}-{2}", created.Year, created.Month, created.Day) + @"</text>
        <!-- IMPORTANT INFORMATION TEXT -->
        <text x=""730"" style=""font-size:12px;fill:#babdb6; text-anchor: end;"">
          <tspan id=""tspan23441"">Důležité doplňující informace si přečtěte na <tspan style=""fill:#729fcf;"">" + link + @"</tspan>
          </tspan>
        </text>
      </g>
";
        }

        private string headerTitle(string title)
        {
            return "<text y=\"32\" x=\"7\" style=\"fill: #000; font-size: 12px; font-weight: bold; text-anchor: start; font-family: sans-serif;\">" + title + "</text>";
        }

        private string headerHours()
        {
            return @"
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
";
        }

        private string headerDays()
        {
            return @"
<rect height=""1"" width=""725"" y=""57"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
<rect height=""1"" width=""725"" y=""107"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
<text y=""85"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Pondělí</text>
<rect height=""1"" width=""725"" y=""157"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
<text y=""135"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans-serif;"">Úterý</text>
<rect height=""1"" width=""725"" y=""207"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
<text y=""185"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Středa</text>
<rect height=""1"" width=""725"" y=""257"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
<text y=""235"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans;"">Čtvrtek</text>
<rect height=""1"" width=""725"" y=""307"" x=""7"" class=""categoryGrid"" fill=""#ddd""/>
<text y=""285"" x=""7"" style=""fill: #000; dominant-baseline: middle; font-size: 12px; text-anchor: start; font-family: sans-serif;"">Pátek</text>
";
        }
    }
}
