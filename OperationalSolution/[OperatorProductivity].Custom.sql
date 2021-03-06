
/* Distinct Website Stored Procedure*/
create proc [dbo].[DistinctWebsites]
as
begin
	select distinct Website from dbo.Conversation with (nolock)
end

go
/* Distinct Devices Stored Procedure*/
create proc [dbo].[DistinctDevices]
as
begin
	select distinct Device from dbo.Visitor with (nolock)
end
go


/****** Object:  StoredProcedure [dbo].[OperatorProductivityFiltered]    Script Date: 04/14/2017 02:38:35 ******/
ALTER PROCEDURE [dbo].[OperatorProductivityFiltered]
@pWebsite nvarchar(50), --inside conversation
@pDevice nvarchar(50),--inside visitor
@pTo varchar(50), --inside converstion 
@pFrom varchar(50) --inside conversation

AS
BEGIN

--declare @pWebsite nvarchar(50) = 'Gucci',
--@pDevice nvarchar(50) ='Mobile',
--@pTo varchar(50) = '20170315',
--@pFrom varchar(50) = '20170302'

--select CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) from conversation c

SELECT OperatorID, Name,

--Proactive Sent,
	(SELECT COUNT(*) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M 
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.OperatorID
					AND M.MessageFrom = 'Operator'
					And C.Website = @pWebsite
					And CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) between @pFrom and @pTo
					--And CONVERT(VARCHAR(23), CAST(c.EndDate AS DATETIME), 112) = @pTo
					And c.VisitorID in (Select V.VisitorID from Visitor V where V.Device = @pDevice)
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM WHERE IM.ConversationID = M.ConversationID)
					))As ProactiveSent,

--Proactive Answered,
	(SELECT COUNT(*) FROM Conversation C  with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M with (nolock)
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.OperatorID
					AND M.MessageFrom = 'Operator'
					And C.Website = @pWebsite
					And CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) between @pFrom and @pTo
					--And CONVERT(VARCHAR(23), CAST(c.EndDate AS DATETIME), 112) = @pTo
					And c.VisitorID in (Select V.VisitorID from Visitor V where V.Device = @pDevice)
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					AND Exists (SELECT 1 FROM Messages VM with (nolock) WHERE VM.MessageID > M.MessageID AND VM.ConversationID = M.ConversationID AND VM.MessageFrom = 'Visitor')
					)) As ProactiveAnswered,

--Reactive Received
	(SELECT COUNT(*) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M with (nolock)
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.VisitorID
					AND M.MessageFrom = 'Visitor'
					And C.Website = @pWebsite
					And CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) between @pFrom and @pTo
					--And CONVERT(VARCHAR(23), CAST(c.EndDate AS DATETIME), 112) = @pTo
					And c.VisitorID in (Select V.VisitorID from Visitor V where V.Device = @pDevice)
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					)) As ReactiveReceived,

--Reactive Answered
	(SELECT COUNT(*) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M with (nolock)
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.VisitorID
					AND M.MessageFrom = 'Visitor'
					And C.Website = @pWebsite
					And CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) between @pFrom and @pTo
					--And CONVERT(VARCHAR(23), CAST(c.EndDate AS DATETIME), 112) = @pTo
					And c.VisitorID in (Select V.VisitorID from Visitor V where V.Device = @pDevice)
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					AND Exists (SELECT 1 FROM Messages VM with (nolock) WHERE VM.MessageID > M.MessageID AND VM.ConversationID = M.ConversationID AND VM.MessageFrom = 'Operator')
					)) As ReactiveAnswered,

--Total Chat Length
	(SELECT IsNull(SUM(DATEDIFF(minute, EndDate, StartDate)),0) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID
		And C.Website = @pWebsite
					And CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) between @pFrom and @pTo
					--And CONVERT(VARCHAR(23), CAST(c.EndDate AS DATETIME), 112) = @pTo
					And c.VisitorID in (Select V.VisitorID from Visitor V where V.Device = @pDevice) ) As TotalChatLengthTime,
--Average Chat Length
	(SELECT IsNull(CONVERT(varchar, AVG(DATEDIFF(minute, StartDate, EndDate))) + 'm' ,0) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID
		And C.Website = @pWebsite
					And CONVERT(VARCHAR(23), CAST(c.StartDate AS DATETIME), 112) between @pFrom and @pTo
					--And CONVERT(VARCHAR(23), CAST(c.EndDate AS DATETIME), 112) = @pTo
					And c.VisitorID in (Select V.VisitorID from Visitor V where V.Device = @pDevice) ) As AverageChatLength

FROM Operator O

END

go