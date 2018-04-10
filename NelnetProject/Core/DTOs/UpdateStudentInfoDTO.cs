﻿using System.Collections.Generic;

namespace Core.DTOs
{
    /// <summary>
    /// Contains information about what student objects need to be updated
    /// </summary>
    public class UpdateStudentInfoDTO
    {
        /// <summary>
        /// The UserID that the students are associated with
        /// </summary>
        public int UserID { get; set; }

        /// <summary>
        /// List of students that have only had their properties updated
        /// </summary>
        public List<Student> UpdatedStudents { get; set; }

        /// <summary>
        /// List of StudentIDs that have been deleted by the user
        /// </summary>
        public List<int> DeletedStudentIDs { get; set; }

        /// <summary>
        /// New students that have been added to the user
        /// </summary>
        public List<Student> AddedStudents { get; set; }
    }
}