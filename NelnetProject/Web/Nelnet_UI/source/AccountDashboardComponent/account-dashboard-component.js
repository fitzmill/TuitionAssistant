﻿require('./account-dashboard-component.scss');
require('../assets/background-image.scss');

const accountDashboardAPIURL = "/api/account-dashboard";
var studentsList = [{ FirstName: "Joey", LastName: "Jimson", Grade: 1 }, { FirstName: "Jimbo", LastName: "Jimson", Grade: 5 }];
var user = {UserID: 1, FirstName: "Jim", LastName: "Jimson", Email: "jimjimson@jimmail.jim", Students: studentsList};

//GETs a user's payment spring information
function getPaymentSpringInfo(userID) {
    let userIDString = user.UserID.toString();
    return $.ajax(accountDashboardAPIURL + "/GetPaymentInfoForUser/" + userIDString, {
        method: "GET"
    });
}

//POSTs any changes to the user
function updatePersonalAndStudentInfo() {
    return $.ajax(accountDashboardAPIURL + "/UpdatePersonalAndStudentInfo", {
        method: "POST",
        data: user
    });
}

//POSTs any changes to the payment info
function updatePaymentInfo(paymentInfo) {
    return $.ajax(accountDashboardAPIURL + "/UpdatePaymentInfo", {
        method: "POST",
        data: paymentInfo
    });
}

ko.components.register('account-dashboard-component', {
    viewModel: function (params) {
        let accountDashboardVM = this;

        accountDashboardVM.CardFirstName = ko.observable();
        accountDashboardVM.CardLastName = ko.observable();
        accountDashboardVM.StreetAddress1 = ko.observable();
        accountDashboardVM.StreetAddress2 = ko.observable();
        accountDashboardVM.City = ko.observable();
        accountDashboardVM.State = ko.observable();
        accountDashboardVM.Zip = ko.observable();
        accountDashboardVM.CardNumber = ko.observable();
        accountDashboardVM.ExpirationYear = ko.observable();
        accountDashboardVM.ExpirationMonth = ko.observable();
        accountDashboardVM.CardType = ko.observable();

        accountDashboardVM.UserFirstName = ko.observable();
        accountDashboardVM.UserLastName = ko.observable();
        accountDashboardVM.Email = ko.observable();
        accountDashboardVM.PaymentPlan = ko.observable();
        accountDashboardVM.Students = ko.observableArray([]);

        accountDashboardVM.NextPaymentDate = ko.observable();
        accountDashboardVM.NextPaymentCost = ko.observable();

        accountDashboardVM.setUser = function () {
            accountDashboardVM.UserFirstName = user.FirstName;
            accountDashboardVM.UserLastName = user.LastName;
            accountDashboardVM.Email = user.Email
            user.Students.forEach(function (student) {
                accountDashboardVM.Students.push(student);
            });
        };

        accountDashboardVM.setUser();

        accountDashboardVM.updateUser = function () {
            user.FirstName = accountDashboardVM.UserFirstName;
            user.LastName = accountDashboardVM.UserLastName;
            user.Email = accountDashboardVM.Email;
            user.Students = [];
            accountDashboardVM.Students.forEach(function (student) {
                user.Students.push(student);
            });

            updatePersonalAndStudentInfo().done({

            }).fail(function (jqXHR) {
                let errorMessage = JSON.parse(jqXHR.responseText).Message;
                window.alert("Could not save information: ".concat(errorMessage));
            });
        };

        accountDashboardVM.updatePaymentInfo = function () {
            var changedPaymentInfo = {
                FirstName: accountDashboardVM.CardFirstName,
                LastName: accountDashboardVM.CardLastName,
                StreetAddress1: accountDashboardVM.StreetAddress1,
                StreetAddress2: accountDashboardVM.StreetAddress2,
                City: accountDashboardVM.City,
                State: accountDashboardVM.State,
                Zip: accountDashboardVM.Zip,
                CardNumber: accountDashboardVM.CardNumber,
                ExpirationYear: accountDashboardVM.ExpirationYear,
                ExpirationMonth: accountDashboardVM.ExpirationMonth,
                CardType: accountDashboardVM.CardType
            };

            updatePaymentInfo(changedPaymentInfo).done({

            }).fail(function (jqXHR) {
                let errorMessage = JSON.parse(jqXHR.responseText).Message;
                window.alert("Could not save information: ".concat(errorMessage));
            });
        }

        getPaymentSpringInfo(user.UserID).done(function (data) {
            accountDashboardVM.CardFirstName = data.FirstName;
            accountDashboardVM.CardLastName = data.LastName;
            accountDashboardVM.StreetAddress1 = data.StreetAddress1;
            accountDashboardVM.StreetAddress2 = data.StreetAddress2;
            accountDashboardVM.City = data.City;
            accountDashboardVM.State = data.State;
            accountDashboardVM.Zip = data.Zip;
            accountDashboardVM.CardNumber = data.CardNumber;
            accountDashboardVM.ExpirationYear = data.ExpirationYear;
            accountDashboardVM.ExpirationMonth = data.ExpirationMonth;
            accountDashboardVM.CardType = data.CardType;
        }).fail(function (jqXHR) {
            window.alert("Could not get payment information, please try refreshing the page.");
        });

        getPaymentSpringInfo(user.UserID);

        return accountDashboardVM;
    },

    template: require('./account-dashboard-component.html')
});