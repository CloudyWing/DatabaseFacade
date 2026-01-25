Imports Microsoft.VisualBasic

Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Data
Imports System.Data.OracleClient
Imports System.Diagnostics
Imports System.Web.Configuration

Imports log4net
Imports log4net.Config

''' <summary>
''' 資料表的CRUD相關處理
''' </summary>
''' <remarks>覺得DBTool太難用了，所以重寫，之後慢慢把DBTool功能搬過來</remarks>
Public NotInheritable Class DataHelper

    Private mConnectionTimeout As Integer = 30
    Private mExecutionTime As TimeSpan
    Private mParameters As New ParameterCollection()

    Public Sub New()
    End Sub

    Public Sub New(sql As String)
        CommandText = sql
    End Sub

    Public Property ConnectionTimeout As Integer
        Get
            Return mConnectionTimeout
        End Get
        Set(value As Integer)
            mConnectionTimeout = value
        End Set
    End Property

    Public ReadOnly Property Connection As OracleConnection
        Get
            Dim conn As OracleConnection = New OracleConnection(WebConfigurationManager.ConnectionStrings("ConnStr").ConnectionString)
            conn.Open()
            Return conn
        End Get
    End Property

    Public Property CommandText As String

    Public ReadOnly Property RealCommandText As String
        Get
            Dim sql As String = CommandText
            For Each parameter As OracleParameter In Parameters
                Dim name As String = If(
                    parameter.ParameterName.StartsWith(":"),
                    parameter.ParameterName,
                    ":" & parameter.ParameterName
                )
                Dim value As String = If(
                    TypeOf parameter.Value Is Date,
                    String.Format(
                        "TO_DATE('{0}', '{1}')",
                        CType(parameter.Value, Date).ToString("yyyy/MM/dd HH:mm:ss"), "yyyy/mm/dd hh24:mi:ss"
                    ),
                    String.Format("'{0}'", parameter.Value)
                )
                Dim pattern As String = String.Format("{0}(?=[\W])|{0}$", name)
                sql = Regex.Replace(sql, pattern, value, RegexOptions.IgnoreCase)
            Next
            Return sql
        End Get
    End Property


    Public Property ExecutionTime As TimeSpan
        Get
            Return mExecutionTime
        End Get
        Private Set(value As TimeSpan)
            mExecutionTime = value
            If value.TotalSeconds > 3.0R Then
                LogManager.GetLogger("SqlOverTime").Warn(value.ToString + "秒" + RealCommandText)
            End If
        End Set
    End Property

    Public ReadOnly Property Parameters As ParameterCollection
        Get
            If (mParameters Is Nothing) Then
                mParameters = New ParameterCollection()
            End If
            Return mParameters
        End Get
    End Property

    Public ReadOnly Property SqlCommand As OracleCommand
        Get
            Dim cmd As New OracleCommand(CommandText)
            cmd.CommandTimeout = ConnectionTimeout
            cmd.Parameters.AddRange(Parameters.CopyToArray())
            Return cmd
        End Get
    End Property

    Public ReadOnly Property DataReader As OracleDataReader
        Get
            Try
                Return GetDataReader(CommandBehavior.CloseConnection)
            Catch
                Throw
            End Try
        End Get
    End Property

    Public ReadOnly Property DataAdapter As OracleDataAdapter
        Get
            Try
                Using cmd As OracleCommand = SqlCommand
                    Return New OracleDataAdapter(cmd)
                End Using
            Catch
                Throw
            End Try
        End Get
    End Property

    Public ReadOnly Property DataTable As DataTable
        Get
            Try
                Using conn As OracleConnection = Connection
                    Dim sw As New Stopwatch()
                    sw.Start()

                    Dim dt As New DataTable()
                    Dim adapter As OracleDataAdapter = DataAdapter
                    adapter.SelectCommand.Connection = conn
                    adapter.Fill(dt)

                    sw.Stop()
                    ExecutionTime = sw.Elapsed
                    Return dt
                End Using
            Catch
                Debug.WriteLine("Error Sql：" & RealCommandText)
                LogManager.GetLogger(Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error("Error Sql：" & RealCommandText)
                Throw
            End Try
        End Get
    End Property

    Public ReadOnly Property Count As Integer
        Get
            Try
                Using conn As OracleConnection = Connection
                    Using cmd As OracleCommand = SqlCommand
                        Dim sw As Stopwatch = New Stopwatch()
                        sw.Start()

                        cmd.CommandText = String.Format(
                            "SELECT COUNT(1) FROM ({0}) Count{1}",
                            CommandText,
                            New Random().Next(100, 999)
                        )
                        cmd.Connection = Connection
                        Dim num As Integer = Convert.ToInt32(cmd.ExecuteScalar())

                        Return num
                    End Using
                End Using
            Catch
                Debug.WriteLine("Error Sql：" & RealCommandText)
                LogManager.GetLogger(Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error("Error Sql：" & RealCommandText)
                Throw
            End Try
        End Get
    End Property

    Public ReadOnly Property HasData As Boolean
        Get
            Try
                Return 0 < Count
            Catch
                Throw
            End Try
        End Get
    End Property

    Public ReadOnly Property DataScalar As Object
        Get
            Try
                Using conn As OracleConnection = Connection

                    Using cmd As OracleCommand = SqlCommand

                        Dim sw As Stopwatch = New Stopwatch()
                        sw.Start()

                        cmd.Connection = conn
                        Dim o As Object = cmd.ExecuteScalar()

                        sw.Stop()
                        ExecutionTime = sw.Elapsed

                        Return o
                    End Using
                End Using
            Catch
                Debug.WriteLine("Error Sql：" & RealCommandText)
                LogManager.GetLogger(Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error("Error Sql：" & RealCommandText)
                Throw
            End Try
        End Get
    End Property

    Public Function GetDataReader(behavior As CommandBehavior) As OracleDataReader

        Dim sw As New Stopwatch()
        sw.Reset()

        Dim conn As OracleConnection = Connection
        Dim cmd As OracleCommand = SqlCommand
        Try
            cmd.Connection = conn
            Dim dr As OracleDataReader = cmd.ExecuteReader(CommandBehavior.CloseConnection Or behavior)

            sw.Stop()
            ExecutionTime = sw.Elapsed

            Return dr
        Catch
            cmd.Dispose()
            If (conn.State = ConnectionState.Open) Then
                conn.Close()
                conn.Dispose()
            End If
            Debug.WriteLine("Error Sql：" & RealCommandText)
            LogManager.GetLogger(Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error("Error Sql：" & RealCommandText)
            Throw
        End Try
    End Function

    Public Function ExecuteNonQuery() As Integer
        Try
            Dim sw As New Stopwatch()
            sw.Start()
            Using conn As OracleConnection = Connection
                Using cmd As OracleCommand = SqlCommand
                    cmd.Connection = conn
                    Dim count As Integer = cmd.ExecuteNonQuery()

                    sw.Stop()
                    ExecutionTime = sw.Elapsed

                    Return count
                End Using
            End Using
        Catch
            Debug.WriteLine("Error Sql：" & RealCommandText)
            LogManager.GetLogger(Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error("Error Sql：" & RealCommandText)
            Throw
        End Try
    End Function

    Public Function ExecuteOracleNonQuery(ByRef rowID As String) As Integer

        Try
            Dim sw As New Stopwatch()
            sw.Start()

            Using conn As OracleConnection = Connection
                Using cmd As OracleCommand = SqlCommand
                    cmd.Connection = conn
                    Dim count As Integer = cmd.ExecuteOracleNonQuery(rowID)

                    sw.Stop()
                    ExecutionTime = sw.Elapsed

                    Return count
                End Using
            End Using
        Catch
            Debug.WriteLine("Error Sql：" & RealCommandText)
            LogManager.GetLogger(Reflection.MethodBase.GetCurrentMethod().DeclaringType).Error("Error Sql：" & RealCommandText)
            Throw
        End Try
    End Function

    Public Sub Reset()
        ConnectionTimeout = 30
        CommandText = String.Empty
        Parameters.Clear()
    End Sub

    Public Class ParameterCollection
        Inherits Collection(Of OracleParameter)

        Protected Overloads Sub Add(ByVal ParamArray params() As OracleParameter)
            For Each p As OracleParameter In params
                MyBase.Add(p)
            Next
        End Sub

        Public Overloads Sub Add(name As String, value As Object)
            If (value Is Nothing) Then
                Throw New ArgumentException("欄位" & name & "不可為Nothing")
            End If
            MyBase.Add(New OracleParameter(name, value))
        End Sub

        Public Overloads Sub Add(name As String, value As Object, dbType As OracleType)
            If (value Is Nothing) Then
                Throw New ArgumentException("欄位" & name & "不可為Nothing")
            End If

            Dim p As New OracleParameter(name, dbType)
            p.Value = value

            MyBase.Add(p)
        End Sub

        Public Overloads Function CopyToArray() As OracleParameter()

            Dim collection As New ParameterCollection()

            For Each oldParameter As OracleParameter In Me
                Dim newParameter As New OracleParameter()
                newParameter.Direction = oldParameter.Direction
                newParameter.IsNullable = oldParameter.IsNullable
                newParameter.OracleType = oldParameter.OracleType
                newParameter.ParameterName = oldParameter.ParameterName
                newParameter.Size = oldParameter.Size
                newParameter.SourceColumn = oldParameter.SourceColumn
                newParameter.SourceColumnNullMapping = oldParameter.SourceColumnNullMapping
                newParameter.SourceVersion = oldParameter.SourceVersion
                newParameter.Value = oldParameter.Value

                collection.Add(newParameter)
            Next
            Return collection.ToArray()
        End Function
    End Class

End Class
