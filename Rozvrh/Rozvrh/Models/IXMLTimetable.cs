﻿using System.Collections.Generic;
using Rozvrh.Models.Timetable;

namespace Rozvrh.Models
{
    public interface IXMLTimetable
    {
        TimetableInfo m_timetableInfo { get; set; }
        List<Department> m_departments { get; }
        List<Course> m_courses { get; }
        List<Lecture> m_lectures { get; }
        List<Lecturer> m_lecturers { get; }
        List<Day> m_days { get; }
        List<Time> m_times { get; }
        List<Building> m_buildings { get; }
        List<Classroom> m_classrooms { get; }
        List<Lesson> m_lessons { get; }
        List<DegreeYear> m_degreeyears { get; }
        List<Specialization> m_specializations { get; }
        List<Group> m_groups { get; } 
        List<GroupLessonBinder> m_groupLessonBinder { get; }
    }
}
