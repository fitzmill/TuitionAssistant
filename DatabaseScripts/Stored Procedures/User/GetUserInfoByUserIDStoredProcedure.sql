USE [NelnetPaymentProcessing]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Description:	Get the information associated with the user id
-- =============================================
CREATE PROCEDURE GetUserInfoByUserID
	@UserID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT u.FirstName, u.LastName, u.Email, u.Hashed, u.Salt, u.PaymentPlan, u.UserType, u.CustomerID, s.StudentId, s.FirstName, s.LastName, s.Grade
	FROM [dbo].[User] u LEFT JOIN [dbo].[Student] s ON u.UserID = s.UserID WHERE u.UserID = @UserID
END
