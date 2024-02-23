﻿Imports System.Windows.Input

Namespace VBNetCore.Mvvm.Commands
    ''' <summary>
    ''' A command whose sole purpose is to relay its functionality to other
    ''' objects by invoking delegates. The default return value for the CanExecute
    ''' method is 'true'.  This class does not allow you to accept command parameters in the
    ''' Execute and CanExecute callback methods.
    ''' </summary>
    Public Class AsyncRelayCommand
        Implements ICommand
        Private ReadOnly _execute As Func(Of Task)
        Private ReadOnly _canExecute As Func(Of Boolean)
        Private _task As Task

        ''' <summary>
        ''' Initializes a new instance of the RelayCommand class.
        ''' </summary>
        ''' <paramname="execute">The execution logic.</param>
        ''' <paramname="canExecute">The execution status logic.</param>
        ''' <exceptioncref="T:System.ArgumentNullException">If the execute argument is null.</exception>
        Public Sub New(ByVal execute As Func(Of Task), ByVal canExecute As Func(Of Boolean))
            If execute Is Nothing Then Throw New ArgumentNullException(NameOf(execute))
            _execute = execute
            _canExecute = canExecute
        End Sub
        Public Sub New(ByVal execute As Func(Of Task))
            Me.New(execute, Nothing)
        End Sub

        ''' <summary>
        ''' Occurs when changes occur that affect whether the command should execute.
        ''' </summary>
        Public Custom Event CanExecuteChanged As EventHandler Implements ICommand.CanExecuteChanged
            AddHandler(ByVal value As EventHandler)
                If _canExecute IsNot Nothing Then
                    AddHandler CommandManager.RequerySuggested, value
                End If
            End AddHandler

            RemoveHandler(ByVal value As EventHandler)
                If _canExecute IsNot Nothing Then
                    RemoveHandler CommandManager.RequerySuggested, value
                End If
            End RemoveHandler
            RaiseEvent(ByVal sender As Object, ByVal e As EventArgs)
            End RaiseEvent
        End Event

        ''' <summary>
        ''' Raises the <seecref="CanExecuteChanged"/> event.
        ''' </summary>
        Private Sub RaiseCanExecuteChanged()
            Call CommandManager.InvalidateRequerySuggested()
        End Sub

        ''' <summary>
        ''' Defines the method that determines whether the command can execute in its current state.
        ''' </summary>
        ''' <paramname="parameter">This parameter will always be ignored.</param>
        ''' <returns>
        ''' true if this command can be executed; otherwise, false.
        ''' </returns>
        <DebuggerStepThrough>
        Public Function CanExecute(ByVal parameter As Object) As Boolean Implements ICommand.CanExecute
            Return (_canExecute Is Nothing OrElse _canExecute()) AndAlso (_task Is Nothing OrElse _task.IsCompleted)
        End Function

        ''' <summary>
        ''' Defines the method to be called when the command is invoked.
        ''' </summary>
        ''' <paramname="parameter">This parameter will always be ignored.</param>
        Public Async Sub Execute(ByVal parameter As Object) Implements ICommand.Execute
            _task = _execute()
            RaiseCanExecuteChanged()
            Await _task
            RaiseCanExecuteChanged()
        End Sub
    End Class
End Namespace
