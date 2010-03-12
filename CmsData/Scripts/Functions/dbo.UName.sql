SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[UName] (@pid int)
RETURNS varchar(100)
AS
BEGIN
	-- Declare the return variable here
	declare @name varchar(100)
	
	SELECT  @name = (case when [Nickname]<>'' then [nickname] else [FirstName] end) + ' ' + [LastName]
	FROM         dbo.People
	WHERE     PeopleId = @pid

	return @name

END
GO