﻿using System;
using BusinessNews.Domain.AccountAggregate.Commands;
using BusinessNews.Domain.AccountAggregate.Events;
using BusinessNews.Domain.BillAggregate;
using BusinessNews.Domain.BusinessAggregate;
using BusinessNews.Domain.SubscriptionAggregate;
using CommonDomain;
using GridDomain.CQRS;
using GridDomain.EventSourcing.Sagas;
using GridDomain.EventSourcing.Sagas.StateSagas;

namespace BusinessNews.Domain.Sagas.BuySubscription
{


    public class BuySubscriptionSaga : StateSaga<BuySubscriptionSaga.State,
                                                 BuySubscriptionSaga.Transitions,
                                                 BuySubscriptionSagaStateAggregate,
                                                 SubscriptionOrderedEvent>
    {
        public enum State
        {
            SubscriptionSet,
            SubscriptionCreating,
            BillPaying,
            BillCreating,
            SubscriptionSetting,
            ChargingSubscription,
            Fault

        }

        public enum Transitions
        {
            CreateBill,
            CreateSubscription,
            PayBill,
            ChangeSubscription,
            SubscriptionSet,
            BillScubscription,
            CreateSubscriptionFault,
            ChargeSubscriptionFault,
            CreateBillFault,
            PayForBillFault,
            ChangeSubscriptionFault
        }

        //TODO: extract subscriptor in separate class
        public static ISagaDescriptor Descriptor => 
            new BuySubscriptionSaga(new BuySubscriptionSagaStateAggregate(Guid.Empty, State.BillCreating));


        public BuySubscriptionSaga(BuySubscriptionSagaStateAggregate state) : base(state)
        {
            ConfigureFaults();

            var subscriptionOrderCreatedTransition = RegisterEvent<SubscriptionOrderedEvent>(Transitions.CreateSubscription);
            var subscriptionOrderCompletedTransition = RegisterEvent<SubscriptionOrderCompletedEvent>(Transitions.SubscriptionSet);

            Machine.Configure(State.SubscriptionSet)
                   .OnEntryFrom(subscriptionOrderCompletedTransition, e => base.State.Complete())
                   .Permit(Transitions.CreateSubscription, State.SubscriptionCreating);
               //    .Permit(Transitions.CreateSubscriptionFault, State.Fault);


            Machine.Configure(State.SubscriptionCreating)
                .OnEntryFrom(subscriptionOrderCreatedTransition,
                    e =>
                    {
                        base.State.RememberOrder(e);
                        Dispatch(new CreateSubscriptionCommand(e.SuibscriptionId, e.OfferId));
                    })
                .Permit(Transitions.BillScubscription, State.ChargingSubscription)
                .Permit(Transitions.CreateSubscriptionFault, State.Fault); 

            var chargeSubscriptionTransition = RegisterEvent<SubscriptionCreatedEvent>(Transitions.BillScubscription);
            Machine.Configure(State.ChargingSubscription)
                .OnEntryFrom(chargeSubscriptionTransition,
                    e =>
                    {
                        Dispatch(new ChargeSubscriptionCommand(e.SubscriptionId, Guid.NewGuid()));
                    })
                .Permit(Transitions.CreateBill, State.BillCreating)
                   .Permit(Transitions.ChargeSubscriptionFault, State.Fault);

            var createBillTransition = RegisterEvent<SubscriptionChargedEvent>(Transitions.CreateBill);
            Machine.Configure(State.BillCreating)
                .OnEntryFrom(createBillTransition,
                    e =>
                    {
                        Dispatch(new CreateBillCommand(new[] {new Charge(e.ChargeId, e.Price)}, Guid.NewGuid()));
                    })
                .Permit(Transitions.PayBill, State.BillPaying)
                .Permit(Transitions.CreateBillFault, State.Fault); 

            var payBillTransition = RegisterEvent<BillCreatedEvent>(Transitions.PayBill);
            Machine.Configure(State.BillPaying)
                .OnEntryFrom(payBillTransition,
                    e => 
                    Dispatch(new PayForBillCommand(base.State.AccountId, e.Amount, e.BillId)))
                .Permit(Transitions.ChangeSubscription, State.SubscriptionSetting)
                .Permit(Transitions.PayForBillFault, State.Fault); ;
            var changeSubscriptionTransition = RegisterEvent<PayedForBillEvent>(Transitions.ChangeSubscription);


            Machine.Configure(State.SubscriptionSetting)
                .OnEntryFrom(changeSubscriptionTransition,
                    e =>
                        Dispatch(new CompleteBusinessSubscriptionOrderCommand(base.State.BusinessId,base.State.SubscriptionId)))
                .Permit(Transitions.SubscriptionSet, State.SubscriptionSet)
                .Permit(Transitions.ChangeSubscriptionFault, State.Fault); 


        }

        private void ConfigureFaults()
        {
            //TODO: replace with more elegatn solution to combine Dispatch and registergin for faults
            var createSubscriptionFault = RegisterCommandFault<CreateSubscriptionCommand>(Transitions.CreateSubscriptionFault);
            var chargeSubscriptionFault = RegisterCommandFault<ChargeSubscriptionCommand>(Transitions.ChargeSubscriptionFault);
            var changeSubscriptionFault = RegisterCommandFault<CompleteBusinessSubscriptionOrderCommand>(Transitions.ChangeSubscriptionFault);
            var createBillCommandFault  = RegisterCommandFault<CreateBillCommand>(Transitions.CreateBillFault);
            var payForBillCommandFault  = RegisterCommandFault<PayForBillCommand>(Transitions.PayForBillFault);

            Machine.Configure(State.Fault)
                   .OnEntryFrom(createSubscriptionFault, DispatchSagaFault)
                   .OnEntryFrom(chargeSubscriptionFault, DispatchSagaFault)
                   .OnEntryFrom(createBillCommandFault, DispatchSagaFault)
                   .OnEntryFrom(changeSubscriptionFault, DispatchSagaFault)
                   .OnEntryFrom(payForBillCommandFault, DispatchSagaFault);
        }
    }
}