﻿using Core.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Interfaces
{
    /// <summary>
    /// Handles the storage of all user information
    /// </summary>
    interface ISetUserInfoEngine
    {
        /// <summary>
        /// Inserts a new user record into the database with the information contained in the user model
        /// </summary>
        /// <param name="user">The user model to insert</param>
        void InsertPersonalInfo(User user);

        /// <summary>
        /// Updates the user record in the database specified by the userID in the user model
        /// </summary>
        /// <param name="user">The user model to update</param>
        void UpdatePersonalInfo(User user);

        /// <summary>
        /// Delete a user record from the database specified by the userID in the user model
        /// </summary>
        /// <param name="userID"></param>
        void DeletePersonalInfo(int userID);

        /// <summary>
        /// Insert new student records into the database
        /// </summary>
        /// <param name="userID">The id of the user that the students will be associated with</param>
        /// <param name="students">The student records to be inserted</param>
        void InsertStudentInfo(int userID, List<Student> students);

        /// <summary>
        /// Update student records in the database
        /// </summary>
        /// <param name="students">The student records to update</param>
        void UpdateStudentInfo(List<Student> students);

        /// <summary>
        /// Delete the student from the database
        /// </summary>
        /// <param name="studentIDs">The ids of the students to delete</param>
        void DeleteStudentInfo(List<int> studentIDs);

        /// <summary>
        /// Insert new payment info into paymentSpring
        /// </summary>
        /// <param name="userPaymentInfo">The payment info to be stored in paymentSpring</param>
        /// <returns></returns>
        string InsertPaymentInfo(UserPaymentInfoDTO userPaymentInfo);

        /// <summary>
        /// Update the payment info associated with the customerID in paymentSpring
        /// </summary>
        /// <param name="customerID">The id of the record to update</param>
        /// <param name="userPaymentInfo">The information to update in paymentSpring</param>
        void UpdatePaymentInfo(string customerID, UserPaymentInfoDTO userPaymentInfo);

        /// <summary>
        /// Delete the payment information from paymentSpring
        /// </summary>
        /// <param name="customerID">The id of the customer to be deleted</param>
        void DeletePaymentInfo(string customerID);

    }
}