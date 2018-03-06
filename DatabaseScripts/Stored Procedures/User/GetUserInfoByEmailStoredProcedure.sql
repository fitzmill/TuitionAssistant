USE [NelnetPaymentProcessing]
GO
/****** Object:  StoredProcedure [dbo].[GetUserInfoByEmail]    Script Date: 2/19/2018 9:15:15 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetUserInfoByEmail
	-- Add the parameters for the stored procedure here
	@Email varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT u.UserID, u.FirstName, u.LastName, u.Hashed, u.Salt, u.PaymentPlan, u.UserType, u.CustomerID, s.StudentId, s.FirstName, s.LastName, s.Grade
	FROM [dbo].[User] u JOIN [dbo].[Student] s ON u.UserID = s.UserID WHERE u.Email = @Email
END
