﻿using System;

namespace Core.Models
{
    /// <summary>
    /// Model for report information
    /// </summary>
    public class Report
    {
        /// <summary>
        /// ID of the Report
        /// </summary>
        public int ReportID { get; set; }

        /// <summary>
        /// Date Of Report Creation
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// Report Start Date
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Report End Date
        /// </summary>
        public DateTime EndDate { get; set; }
    }
}
