using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChickenBusiness.EncoderDecoderService;

namespace ChickenBusiness
{
    class EncodeDecodeOrder
    {
        // service client for EncoderDecoderService referance
        public static EncoderDecoderService.ServiceClient coderServiceClient = new ServiceClient();

        // encode an Order object to a string
        public static string EncodeOrder(OrderClass order)
        {
            return coderServiceClient.Encrypt(order.senderId.ToString() + "," 
                + order.cardNo.ToString() + "," + order.amount.ToString() + "," + order.timeOfOrder.ToString());
        }

        // decode a given string to Order object
        public static OrderClass DecodeOrder(string encodedOrder)
        {
            OrderClass orderReceived = new OrderClass();
            string[] decodedOrder = coderServiceClient.Decrypt(encodedOrder).Split(',');

            if(decodedOrder.Length == 4)
            {
                orderReceived.senderId = Convert.ToInt32(decodedOrder[0]);
                orderReceived.cardNo = Convert.ToInt32(decodedOrder[1]);
                orderReceived.amount = Convert.ToInt32(decodedOrder[2]);
                orderReceived.timeOfOrder = Convert.ToDateTime(decodedOrder[3]);
            }
            else
            {
                Console.WriteLine("Error in decoding order!");
            }

            return orderReceived;
        }
    }
}
