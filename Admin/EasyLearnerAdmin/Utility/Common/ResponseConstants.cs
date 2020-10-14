using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyLearnerAdmin.Utility.Common
{
    public static class ResponseConstants
    {
        //Common
        public const string SomethingWrong = "Something went wrong !";

        //Grade
        public const string CreateNewGrade = "Grade Created Successfully";
        public const string UpdateGrade = "Grade Updated Successfully";
        public const string DeleteGrade = "Grade Deleted Successfully";

        //Exam
        public const string CreateNewExam = "Exam Created Successfully";
        public const string UpdateExam = "Exam Updated Successfully";
        public const string DeleteExam = "Exam Deleted Successfully";

        //ExamQuestion
        public const string CreateNewExamQuestion = "ExamQuestion Created Successfully";
        public const string UpdateExamQuestion = "ExamQuestion Updated Successfully";
        public const string DeleteExamQuestion = "ExamQuestion Deleted Successfully";

        //Lesson
        public const string CreateNewLesson = "Lesson Created Successfully";
        public const string UpdateLesson = "Lesson Updated Successfully";
        public const string DeleteLesson = "Lesson Deleted Successfully";

         //Friends   
        public const string CreateInvitaionCode = "Invitation Code Created Successfully";
        public const string UpdateInvitationCode = "Invitation Code Updated Successfully";



        //Subscription
        public const string CreateSubscription = "Membership service Created Successfully";
        public const string UpdateSubscription = "Membership service Updated Successfully";
        public const string MembershipServiceAlert = "Membership service is exist in system , You can edit this existing membership service from the edit page";

        //Tutor
        public const string CreateNewTutor = "Tutor Creation is successfull.";
        public const string UpdateTutor = "Tutor Updated SuccessFully";
        public const string DeleteTutor = "Tutor Deleted  SuccessFully";
        public const string UpdateTutorStatus = "Tutor Status Updated Successfully";
        public const string UpdateTutorName = "Tutor Name Updated SuccessFully";
        public const string UpdateTutorMobileNumber = "Tutor Mobile Number Updated Successfully";
        public const string UpdateTutorPassword = "Tutor Password Updated Successfully";
        public const string UpdateTutorRelevantLesson = "Tutor RelevantLesson Updated Successfully";
        public const string TutorUserExist = "The User name is duplicate.Please try again.";

        //Student
        public const string CreateNewStudent= "Congratulations the creation of student is successfull.";
        public const string UpdateStudent = "Student Updated SuccessFully";
        public const string UpdateStudentName = "Student Name Updated Successfully";
        public const string UpdateStudentGrade = "Student Grade Updated Successfully";
        public const string UpdateStudentPassword = "Student Password Updated Successfully";
        public const string DeleteStudent = "Student Deleted Successfully";
        public const string UserExist = "The Username you enter is duplicate in system.Please Enter new user name.";
        public const string MobileNoExist = "The Mobile no is duplicate. Please try again";
       
       

        //Message
        public const string CreateMessage = "Message Send successfully";

        //financial
        public const string ReportFromMembership = "Report From Membership";
        public const string PaymentRegistartion = "Payment Registartion";
        public const string ReportPaymentHistory = "Report Payment History";
        public const string ReportTotalPaymentToTutor = "Report Total Payment To Tutor";
        public const string ReportTotalPaymentToStaff = "Report Total Payment To Staff";
        public const string SuccessPayment = "Payment Added Successfully";

        //Membership
        public const string StudentMembeshipNoOfQuestionUpdated = "Student subscription for NoOfQuestion Updated Successfully";
        public const string StudentMembeshipNoOfDaysUpdated = "Student subscription for NoOfDays Updated Successfully";
        public const string StudentMembeshipStatusUpdated = "Student subscription for membership status Updated Successfully";


        //Staff
        public const string DeleteStaff = "Staff Deleted Successfully";
        public const string DeactivatedStaff = "Staff Deactivated Successfully";
        public const string ActivatedStaff = "Staff Activated Successfully";
        public const string StaffDeletionError = "Something went wrong while removing staff";

        //Setting
        public const string SettingTextCreated = "Setting Text added successfully";
        public const string SettingTextUpdated = "Settings Text updated successfully";

        //Support
        public const string RespondToSupportRequest = "Responded to the SupportRequest";

    }
}
