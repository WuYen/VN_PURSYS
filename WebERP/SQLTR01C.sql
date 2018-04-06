select TR01A_ID, count(AllArrive) as NotReceive from
(
select A.TR01A_ID, A.TR01B_ID,
	CASE (A.PUR_QT) WHEN SUM(B.ARR_QT) THEN 'T' ELSE 'F' END AllArrive
	from TR01B as A
left join TR01C as B on A.TR01B_ID = B.TR01B_ID
where A.TR01A_ID = 26  
group by A.TR01A_ID, A.TR01B_ID, A.PUR_QT
) as temp
where AllArrive = 'F'
group by TR01A_ID


select [dbo].[Get_Not_Receive_Count](23)

select * from TR01A
where CFN_YN = 'P'

select * from TR01C
where TR01B_ID = 26

--update TR01A 
--set CFN_YN = 'N'
--where CFN_YN IS NULL and TR01A_ID = 27;

select * from BA02A
where ITM_NO like '%HNO3%'

select * from TR01B
where BA02A_ID = 1060

SELECT A.TR01B_ID, Sum(B.ARR_QT) TotalQT , Sum(B.INV_MY) TotalMY
FROM[DB_A2BD6B_PandY].[dbo].[TR01B] as A 
left join[DB_A2BD6B_PandY].[dbo].[TR01C] as B 
on A.TR01B_ID = B.TR01B_ID 
where A.TR01A_ID = 27 group by A.TR01B_ID
