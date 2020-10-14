using EasyLearner.Service.Dto;
using EasyLearner.Service.Interface.BaseInterface;
using EasyLearnerAdmin.Data.DbModel;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EasyLearner.Service.Interface
{
    public interface IPaymentService : IGenericService<Payments>
    {
        Task<List<PaymentHistoryDto>> GetPaymentByTutorList(SqlParameter[] paraObjects);
        Task<List<PaymentHistoryDto>> GetPaymentByStaffList(SqlParameter[] paraObjects);
    }
}
