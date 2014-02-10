using Rozvrh.Exporters.Common;
using Rozvrh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Rozvrh.Controllers
{
    /// <summary>
    ///  Helper class for dividing lectures to groups for rendering and export to svg
    /// </summary>
    public class LecturesGroupDivider
    {
        /// <summary> 
        /// Divides list of ExportLecture(s) to groups of overlapping lectures.
        /// </summary> 
        /// <returns> List of groups (group = another list) of ExportLecture(s) </returns>
        /// <param name="lectures">List of ExportLecture(s) to divide.</param>
        public List<List<ExportLecture>> divideToGroups(List<ExportLecture> lectures)
        {
            List<List<ExportLecture>> groups = new List<List<ExportLecture>>();
            groups.Add(new List<ExportLecture>());
            foreach (ExportLecture l in lectures)
            {
                var lastgroup = groups[groups.Count - 1];
                if (lastgroup.Count < 1)
                {
                    groups[groups.Count - 1].Add(l);
                }
                //Is lecture l starting sooner than end time of the last lecture in last group (with 10 minutes toleration)?
                else if (DateTime.Compare(l.StartTime, lastgroup.Max(lec => lec.StartTime + lec.Length - new TimeSpan(0, 10, 0))) < 0
                    && l.Day == lastgroup[lastgroup.Count - 1].Day)
                {
                    //All lectures intersecting with l
                    IEnumerable<ExportLecture> intersecting = lastgroup.Where(lec => (DateTime.Compare(l.StartTime, lec.StartTime + lec.Length - new TimeSpan(0, 10, 0)) < 0));
                    //Only have to worry about 3 and more lectures in one group
                    //If lecture l is intersecting with all of lectures in lastgroup then it belongs to the group
                    if (lastgroup.Count <= 1 || intersecting.Count() == lastgroup.Count)
                    {
                        lastgroup.Add(l);
                    }
                    //Otherwise create a new group with fake lectures
                    else
                    {
                        List<ExportLecture> newGroup = new List<ExportLecture>();
                        //Fake lectures from all last group lectures
                        foreach (ExportLecture lastGroupLec in lastgroup)
                        {

                            ExportLecture fakeLecture = null;
                            //Treat intersecting lectures as not null
                            if (intersecting.Contains(lastGroupLec))
                            {
                                fakeLecture = new ExportLecture(null, lastGroupLec.Day, lastGroupLec.StartTime, lastGroupLec.Length, null, null, null, true);
                            }
                            //Non-intersecting lecture are null = empty slots
                            newGroup.Add(fakeLecture);
                        }

                        //Find first empty slot in newGroup (null lecture) and swap with lecture l
                        int index = newGroup.FindIndex(lecture => lecture == null);
                        newGroup[index] = l;
                        //Finally remove all empty slots and add the newGroup to the groups
                        newGroup.RemoveAll(lecture => lecture == null);
                        groups.Add(newGroup);
                        lastgroup = newGroup;
                    }

                }
                else
                {
                    groups.Add(new List<ExportLecture>());
                    lastgroup = groups[groups.Count - 1];
                    lastgroup.Add(l);
                }
            }
            return groups;
        }

        /// <summary> 
        /// Divides list ofTimetableField(s) to groups of overlapping lectures.
        /// </summary> 
        /// <returns> List of groups (group = another list) of TimetableField(s)  </returns>
        /// <param name="lectures">List of TimetableField(s) to divide.</param>
        public List<List<Models.TimetableField>> divideToGroups(List<Models.TimetableField> fields)
        {
            fields.Sort((x, y) => DateTime.Compare(getStartTime(x), getStartTime(y)));
            var groups = new List<List<TimetableField>> { new List<TimetableField>() };

            foreach (TimetableField l in fields)
            {
                var lastgroup = groups[groups.Count - 1];
                if (lastgroup.Count < 1)
                {
                    groups[groups.Count - 1].Add(l);
                }
                //Is lecture l starting sooner than end time of the last lecture in last group (with 10 minutes toleration)?
                else if (DateTime.Compare(getStartTime(l), lastgroup.Max(lec => getStartTime(lec) + getDuration(lec) - new TimeSpan(0, 10, 0))) < 0)
                {

                    //All lectures intersecting with l
                    List<TimetableField> intersecting = lastgroup.Where(lec => (DateTime.Compare(getStartTime(l), getStartTime(lec) + getDuration(lec) - new TimeSpan(0, 10, 0)) < 0)).ToList();
                    //Only have to worry about 3 and more lectures in one group
                    //If lecture l is intersecting with all of lectures in lastgroup then it belongs to the group
                    if (lastgroup.Count <= 1 || intersecting.Count() == lastgroup.Count)
                    {
                        lastgroup.Add(l);
                    }
                        //Otherwise create a new group with fake lectures
                    else
                    {
                        List<TimetableField> newGroup = new List<TimetableField>();
                        //Fake lectures from all last group lectures
                        foreach (TimetableField lastGroupLec in lastgroup)
                        {

                            TimetableField fakeLecture = null;
                            //Treat intersecting lectures as not null
                            if (intersecting.Contains(lastGroupLec))
                            {
                                
                                fakeLecture = (TimetableField)new FakeTimetableField(lastGroupLec.time_hours, lastGroupLec.time_minutes, lastGroupLec.duration);
                           
                            }
                            //Non-intersecting lecture are null = empty slots
                            newGroup.Add(fakeLecture);
                        }

                        //Find first empty slot in newGroup (null lecture) and swap with lecture l
                        int index = newGroup.FindIndex(lecture => lecture == null);
                        newGroup[index] = l;
                        //Finally remove all empty slots and add the newGroup to the groups
                        newGroup.RemoveAll(lecture => lecture == null);
                        groups.Add(newGroup);
                    }
                }
                else
                {
                    groups.Add(new List<TimetableField>());
                    lastgroup = groups[groups.Count - 1];
                    lastgroup.Add(l);
                }
            }
            return groups;
        }

        /// <summary>
        /// Returns 1.1.2001 date with hours and minutes of the TimetableField
        /// </summary>
        /// <param name="field">Field to be processed</param>
        /// <returns></returns>
        private static DateTime getStartTime(TimetableField field)
        {
            return new DateTime(2001, 1, 1, int.Parse(field.time_hours), int.Parse(field.time_minutes), 0);
        }

        /// <summary>
        /// Returns fields duration
        /// </summary>
        /// <param name="field">Field to be processed</param>
        /// <returns></returns>
        private static TimeSpan getDuration(TimetableField field)
        {
            return new TimeSpan(int.Parse(field.duration), 0, 0);
        }
    }
}