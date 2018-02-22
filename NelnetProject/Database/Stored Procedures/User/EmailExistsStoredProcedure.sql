USE [NelnetPaymentProcessing]
GO
/****** Object:  StoredProcedure [dbo].[EmailExists]    Script Date: 2/19/2018 9:22:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE EmailExists 
	-- Add the parameters for the stored procedure here
	@Email varchar(255)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Email FROM [dbo].[User] u WHERE u.Email = @Email
END
