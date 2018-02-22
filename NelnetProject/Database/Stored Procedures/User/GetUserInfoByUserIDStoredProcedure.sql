USE [NelnetPaymentProcessing]
GO
/****** Object:  StoredProcedure [dbo].[GetUserInfoUserID]    Script Date: 2/19/2018 9:15:58 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE GetUserInfoByUserID
	-- Add the parameters for the stored procedure here
	@UserID int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT FirstName, LastName, Email, Hashed, Salt, PaymentPlan, UserType, CustomerID 
	FROM [dbo].[User] u WHERE u.UserID = @UserID
END
