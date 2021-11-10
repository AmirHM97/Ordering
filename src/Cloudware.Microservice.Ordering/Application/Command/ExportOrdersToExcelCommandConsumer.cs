using ClosedXML.Excel;
using Cloudware.Utilities.Contract.Abstractions;
using Cloudware.Utilities.Contract.Notification;
using MassTransit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Cloudware.Microservice.Ordering.Application.Command
{
    public class ExportOrdersToExcelCommandConsumer : IConsumer<ExportOrdersToExcelCommand>, IMediatorConsumerType
    {
        public async Task Consume(ConsumeContext<ExportOrdersToExcelCommand> context)
        {
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            var workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("WithdrawalRequest");

            worksheet.Cell(1, 1).Value = "نام خریدار";
            worksheet.Cell(1, 2).Value = "شماره همراه خریدار";
            worksheet.Cell(1, 3).Value = "کد پستی خریدار";
            worksheet.Cell(1, 4).Value = "ادرس پستی خریدار";
            worksheet.Cell(1, 5).Value = "قیمت کل";
            worksheet.Cell(1, 6).Value = "محصولات";
            worksheet.Cell(1, 7).Value = "نام فروشنده";
            //worksheet.Cell(1, 8).Value = "ادرس فروشنده";
            //worksheet.Cell(1, 9).Value = "شماره فروشنده";

            for (int index = 1; index <= context.Message.Orders.Count; index++)
            {
                worksheet.Cell(index + 1, 1).Value = context.Message.Orders[index - 1].UserFirstName + "" + context.Message.Orders[index - 1].UserLastName;
                worksheet.Cell(index + 1, 2).Value = context.Message.Orders[index - 1].MobileNumber;
                worksheet.Cell(index + 1, 3).Value = context.Message.Orders[index - 1].PostalCode;
                worksheet.Cell(index + 1, 4).Value = context.Message.Orders[index - 1].PostalAddress;
                worksheet.Cell(index + 1, 5).Value = context.Message.Orders[index - 1].TotalPrice;
                worksheet.Cell(index + 1, 6).Value = context.Message.Orders[index - 1].OrderItems.Select(e => e.ProductName + "قیمت:" + e.UnitPrice);
                worksheet.Cell(index + 1, 7).Value = context.Message.Orders[index - 1].OrderItems.Select(e => e.SellerName);
                //worksheet.Cell(index + 1, 8).Value = context.Message.Orders[index - 1].OrderItems.Select(e => e.SellerAddress);
                //worksheet.Cell(index + 1, 9).Value = context.Message.Orders[index - 1].OrderItems.Select(e => e.SellerMobile);
            }

            using (var stream = new MemoryStream())
            {
                workbook.SaveAs(stream);
                var content = stream.ToArray();
                await context.RespondAsync(new ExportOrdersToExcelCommandResponse(content, contentType));
            }
        }

        public class ExportOrdersToExcelCommandResponse
        {
            public ExportOrdersToExcelCommandResponse(byte[] content, string contentType)
            {
                Content = content;
                ContentType = contentType;
            }

            public byte[] Content { get; set; }
            public string ContentType { get; set; }
        }
    }
}
