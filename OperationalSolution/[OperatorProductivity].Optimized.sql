USE [chat]
GO
/****** Object:  StoredProcedure [dbo].[OperatorProductivity]    Script Date: 04/13/2017 23:45:26 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[OperatorProductivity]
AS
BEGIN
SELECT OperatorID, Name,

--Proactive Sent,
	(SELECT COUNT(*) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M 
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.OperatorID
					AND M.MessageFrom = 'Operator'
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					))As ProactiveSent,

--Proactive Answered,
	(SELECT COUNT(*) FROM Conversation C  with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M with (nolock)
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.OperatorID
					AND M.MessageFrom = 'Operator'
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					AND Exists (SELECT 1 FROM Messages VM with (nolock) WHERE VM.MessageID > M.MessageID AND VM.ConversationID = M.ConversationID AND VM.MessageFrom = 'Visitor')
					)) As ProactiveAnswered,

--Proactive Response Rate,
	--CASE WHEN (SELECT COUNT(*) FROM Conversation C 
	--	WHERE C.OperatorID = O.OperatorID 
	--	AND Exists (SELECT 1 FROM Messages M 
	--				WHERE M.ConversationID = C.ConversationID
	--				AND M.MessageUserID = O.OperatorID
	--				AND M.MessageFrom = 'Operator'
	--				)) > 0 THEN

	--(SELECT COUNT(*) FROM Conversation C 
	--	WHERE C.OperatorID = O.OperatorID 
	--	AND Exists (SELECT 1 FROM Messages M 
	--				WHERE M.ConversationID = C.ConversationID
	--				AND M.MessageUserID = O.OperatorID
	--				AND M.MessageFrom = 'Operator'
	--				AND Exists (SELECT 1 FROM Messages VM WHERE VM.MessageID > M.MessageID AND VM.ConversationID = M.ConversationID AND VM.MessageFrom = 'Visitor')
	--				)) / 

	--(SELECT COUNT(*) FROM Conversation C 
	--	WHERE C.OperatorID = O.OperatorID 
	--	AND Exists (SELECT 1 FROM Messages M 
	--				WHERE M.ConversationID = C.ConversationID
	--				AND M.MessageUserID = O.OperatorID
	--				AND M.MessageFrom = 'Operator'
	--				)) ELSE 1 END
	-- * 100 As ProactiveResponseRate,

--Reactive Received
	(SELECT COUNT(*) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M with (nolock)
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.VisitorID
					AND M.MessageFrom = 'Visitor'
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					)) As ReactiveReceived,

--Reactive Answered
	(SELECT COUNT(*) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID 
		AND Exists (SELECT 1 FROM Messages M with (nolock)
					WHERE M.ConversationID = C.ConversationID
					AND M.MessageUserID = C.VisitorID
					AND M.MessageFrom = 'Visitor'
					AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM with (nolock) WHERE IM.ConversationID = M.ConversationID)
					AND Exists (SELECT 1 FROM Messages VM with (nolock) WHERE VM.MessageID > M.MessageID AND VM.ConversationID = M.ConversationID AND VM.MessageFrom = 'Operator')
					)) As ReactiveAnswered,

----Reactive Response Rate
	--CASE When (SELECT COUNT(*) FROM Conversation C 
	--	WHERE C.OperatorID = O.OperatorID 
	--	AND Exists (SELECT 1 FROM Messages M 
	--				WHERE M.ConversationID = C.ConversationID
	--				AND M.MessageUserID = C.VisitorID
	--				AND M.MessageFrom = 'Visitor'
	--				AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM WHERE IM.ConversationID = M.ConversationID)
	--				)) > 0 Then
	
	--(SELECT COUNT(*) FROM Conversation C 
	--	WHERE C.OperatorID = O.OperatorID 
	--	AND Exists (SELECT 1 FROM Messages M 
	--				WHERE C.ConversationID = C.ConversationID
	--				AND M.MessageUserID = C.VisitorID
	--				AND M.MessageFrom = 'Visitor'
	--				AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM WHERE IM.ConversationID = M.ConversationID)
	--				AND Exists (SELECT 1 FROM Messages VM WHERE VM.MessageID > M.MessageID AND VM.ConversationID = M.ConversationID AND VM.MessageFrom = 'Operator')
	--				)) /
	--(SELECT COUNT(*) FROM Conversation C 
	--	WHERE C.OperatorID = O.OperatorID 
	--	AND Exists (SELECT 1 FROM Messages M 
	--				WHERE M.ConversationID = C.ConversationID
	--				AND M.MessageUserID = C.VisitorID
	--				AND M.MessageFrom = 'Visitor'
	--				AND M.MessageID = (SELECT Min(IM.MessageID) FROM Messages IM WHERE IM.ConversationID = M.ConversationID)
	--				)) ELSE 1 End 
	--				* 100 As ReactiveResponseRate,

--Total Chat Length
	(SELECT IsNull(SUM(DATEDIFF(minute, EndDate, StartDate)),0) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID ) As TotalChatLengthTime,
--Average Chat Length
	(SELECT IsNull(CONVERT(varchar, AVG(DATEDIFF(minute, StartDate, EndDate))) + 'm' ,0) FROM Conversation C with (nolock)
		WHERE C.OperatorID = O.OperatorID ) As AverageChatLength

FROM Operator O

END
