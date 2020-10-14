using EasyLearnerAdmin.Data.DbModel.BaseModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace EasyLearnerAdmin.Data.DbModel
{
    public class TutorRelevantLesson 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Required]
        public long TutorId { get; set; }
        [ForeignKey("TutorId")]
        public virtual Tutors Tutor { get; set; }

        [Required]
        public long LessonId { get; set; }
        [ForeignKey("LessonId")]
        public virtual Lessons Lesson { get; set; }
    }
}
