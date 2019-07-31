namespace ExamplePaymentPlugin
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using CommerceBuilder.DomainModel;
    using CommerceBuilder.Payments;
    using CommerceBuilder.Utility;
    using System.IO;
    using System.Reflection;
    using System.Collections;
    using CommerceBuilder.Payments.Providers;

    /// <summary>
    /// This provider provides the test implementations for a payment gateway.
    /// </summary>
    public class EPProvider : CommerceBuilder.Payments.Providers.PaymentProviderBase
    {
        private bool _useAuthCapture = false;
        private GatewayExecutionMode _executionMode = GatewayExecutionMode.AlwaysAccept; 
        
        /// <summary>
        /// Gets or sets the authorize or authorize and capture mode
        /// </summary>
        public bool UseAuthCapture
        {
            get { return _useAuthCapture; }
            set { _useAuthCapture = value; }
        }

        /// <inheritdoc />
        public override string Name
        {
            get { return "ExamplePaymentPlugin"; }
        }

        /// <inheritdoc />
        public override string Description
        {
            get { return "ExamplePaymentPlugin is a dummy gateway for testing purposes only."; }
        }

        /// <inheritdoc />
        public override string GetLogoUrl()
        {
            return "~/Plugins/ExamplePaymentPlugin/Content/Logo.png";
        }

        /// <inheritdoc />
        public override string GetConfigUrl()
        {
            return "~/Admin/Payments/ExamplePaymentPlugin/Config";
        }

        /// <inheritdoc />
        public override string Version
        {
            get { return "1.0"; }
        }

        /// <inheritdoc />
        public override string ConfigReference
        {
            get { return "ExamplePaymentPlugin"; }
        }
                
        /// <summary>
        /// Gets or sets the execution mode
        /// </summary>
        public GatewayExecutionMode ExecutionMode
        {
            get { return _executionMode; }
            set { _executionMode = value; }
        }

        /// <inheritdoc />
        public override SupportedTransactions SupportedTransactions
        {
            get
            {
                return (SupportedTransactions.Authorize
                    | SupportedTransactions.AuthorizeCapture
                    | SupportedTransactions.Capture
                    | SupportedTransactions.PartialCapture
                    | SupportedTransactions.Refund
                    | SupportedTransactions.PartialRefund
                    | SupportedTransactions.Void
                    | SupportedTransactions.RecurringBilling);
            }
        }

        /// <inheritdoc />
        public override void Initialize(int PaymentGatewayId, IDictionary<string, string> ConfigurationData)
        {
            base.Initialize(PaymentGatewayId, ConfigurationData);
            if (ConfigurationData.ContainsKey("ExecutionMode")) ExecutionMode = ParseExecutionMode(ConfigurationData["ExecutionMode"]);
            if (ConfigurationData.ContainsKey("UseAuthCapture")) UseAuthCapture = AlwaysConvert.ToBool(ConfigurationData["UseAuthCapture"], true);
        }
        
        /// <inheritdoc />
        public override Transaction DoAuthorize(AuthorizeTransactionRequest authorizeRequest)
        {
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            if (authorizeRequest.Capture || this.UseAuthCapture)
            {
                return CreateTransaction(TransactionType.AuthorizeCapture, authorizeRequest.Amount);
            }
            else
            {
                return CreateTransaction(authorizeRequest.TransactionType, authorizeRequest.Amount);
            }
        }

        /// <inheritdoc />
        public override Transaction DoCapture(CaptureTransactionRequest captureRequest)
        {
            Payment payment = captureRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            return CreateTransaction(captureRequest.TransactionType, captureRequest.Amount);
        }

        /// <inheritdoc />
        public override Transaction DoRefund(RefundTransactionRequest creditRequest)
        {
            Payment payment = creditRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            return CreateTransaction(creditRequest.TransactionType, creditRequest.Amount);
        }

        /// <inheritdoc />
        public override Transaction DoVoid(VoidTransactionRequest voidRequest)
        {
            Payment payment = voidRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            Transaction authorizeTransaction = voidRequest.AuthorizeTransaction;
            if (authorizeTransaction == null) throw new ArgumentNullException("voidRequest.AuthorizeTransaction");
            return CreateTransaction(voidRequest.TransactionType, voidRequest.Amount);
        }

        /// <inheritdoc />
        public override AuthorizeRecurringTransactionResponse DoAuthorizeRecurring(AuthorizeRecurringTransactionRequest authorizeRequest)
        {
            Payment payment = authorizeRequest.Payment;
            if (payment == null) throw new ArgumentNullException("request.Payment");
            AuthorizeRecurringTransactionResponse resp = new AuthorizeRecurringTransactionResponse();
            Transaction trans;
            if (authorizeRequest.RecurringChargeSpecified)
            {
                trans = CreateTransaction(authorizeRequest.TransactionType, authorizeRequest.Amount);
                resp.Transactions.Add(trans);
            }
            decimal amount = authorizeRequest.RecurringChargeSpecified ? authorizeRequest.RecurringCharge : authorizeRequest.Amount;
            trans = CreateTransaction(authorizeRequest.TransactionType, amount);
            resp.Transactions.Add(trans);
            if (trans.TransactionStatus == TransactionStatus.Successful)
            {
                trans.ProviderTransactionId = Guid.NewGuid().ToString();
            }
            return resp;
        }

        private Transaction CreateTransaction(TransactionType transactionType, decimal transactionAmount)
        {
            //CREATE THE TRANSACTION OBJECT
            Transaction transaction = new Transaction();
            transaction.PaymentGateway = EntityLoader.Load<PaymentGateway>(this.PaymentGatewayId);
            transaction.TransactionType = transactionType;

            bool accept = true;
            if (ExecutionMode == GatewayExecutionMode.AlwaysAccept)
            {
                accept = true;
            }
            else if (ExecutionMode == GatewayExecutionMode.AlwaysReject)
            {
                accept = false;
            }
            else if (ExecutionMode == GatewayExecutionMode.Random)
            {
                // 50% chance of acceptance
                accept = (new Random()).Next(2) == 1;
            }

            // set values common to all transaction types
            transaction.TransactionDate = LocaleHelper.LocalNow;
            transaction.Amount = transactionAmount;
            HttpContext context = HttpContext.Current;
            if (context != null)
            {
                transaction.RemoteIP = context.Request.ServerVariables["REMOTE_ADDR"];
                transaction.Referrer = context.Request.ServerVariables["HTTP_REFERER"];
            }

            // NOT SUPPORTED
            transaction.AVSResultCode = "S";

            // NO RESPONSE
            transaction.CVVResultCode = "X"; 

            // set remaining values depending on whether transaction succeeds
            if (accept)
            {
                // SUCCESSFUL
                transaction.TransactionStatus = TransactionStatus.Successful;
                transaction.ProviderTransactionId = Guid.NewGuid().ToString();
                transaction.ResponseCode = "0";
                transaction.ResponseMessage = "Transaction Successful";
                transaction.AuthorizationCode = StringHelper.RandomNumber(6);
            }
            else
            {
                // FAILED
                transaction.TransactionStatus = TransactionStatus.Failed;
                transaction.ResponseCode = "999";
                transaction.ResponseMessage = "Transaction Failed.";
            }

            return transaction;
        }

        /// <summary>
        /// Gateway execution mode enum
        /// </summary>
        public enum GatewayExecutionMode
        {
            AlwaysAccept=1,
            AlwaysReject=2,
            Random=3
        }

        private GatewayExecutionMode ParseExecutionMode(string value)
        {
            if (string.IsNullOrEmpty(value)) return GatewayExecutionMode.AlwaysAccept;
            if (value.Equals(GatewayExecutionMode.AlwaysAccept.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return GatewayExecutionMode.AlwaysAccept;
            }
            else if (value.Equals(GatewayExecutionMode.AlwaysReject.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return GatewayExecutionMode.AlwaysReject;
            }
            else if (value.Equals(GatewayExecutionMode.Random.ToString(), StringComparison.InvariantCultureIgnoreCase))
            {
                return GatewayExecutionMode.Random;
            }
            else
            {
                return GatewayExecutionMode.AlwaysAccept;
            }
        }
    }
}