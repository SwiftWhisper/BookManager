﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;

namespace BookManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<Book> books = new ObservableCollection<Book>();
        public MainWindow()
        {
			InitializeComponent();
            loadData();
			BooksGataGrid.ItemsSource = books;
		}
		public void loadData()
		{
            books.Clear();
            string connectionString = "server=localhost;port=3306;user=root;password=Hkle4X!5di_3;database=bookmanager;";
            using var connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();
                string query = "SELECT * FROM books;";
                using var cmd = new MySqlCommand(query, connection);
                using var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    books.Add(new Book
                    {
                        code = reader.GetInt32("code"),
                        author = reader.GetString("author"),
                        title = reader.GetString("title"),
                        year = reader.GetInt32("year"),
                        rack = reader.GetInt32("rack"),
                        shelf = reader.GetInt32("shelf")
                    });

                }
            }
            catch (MySqlException sqlEx)
            {
                MessageBox.Show("Не вдалось отрмати доступ до бази даних з інформаціє. Перевірте підключення до мережі та перезавантажте додаток");
                Application.Current.Shutdown();
            }
            if (books == null)
            {
                MessageBox.Show("В базі даних відсутня інформація про книги");
            }
        }
        private void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            AuthWindow authWindow = new AuthWindow();
            authWindow.ShowDialog();
        }

		private void EditButton_Click(object sender, RoutedEventArgs e)
		{
            Button button = sender as Button;

            Book selectedBook = button.DataContext as Book;
            AddEditWindow editWindow = new AddEditWindow(selectedBook);
            if (editWindow.ShowDialog() == true)
            {
                loadData();
            }
        }

		private void AddButton_Click(object sender, RoutedEventArgs e)
		{
			AddEditWindow addWindow = new AddEditWindow();
            if (addWindow.ShowDialog() == true)
            {
               loadData();
            }
        }
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DeleteWindow deleteWindow = new DeleteWindow();
            if (deleteWindow.ShowDialog() == true)
            {
                string connectionString = "server=localhost;port=3306;user=root;password=Hkle4X!5di_3k;database=bookmanager;";
                using var connection = new MySqlConnection(connectionString);
                Button button = sender as Button;
                Book selectedBook = button.DataContext as Book;
                try
                {
                    connection.Open();
                    string deleteQuery = "DELETE FROM books WHERE code = @Code";
                    using var command = new MySqlCommand(deleteQuery, connection);
                    command.Parameters.AddWithValue("@Code", selectedBook.code);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Книгу успішно видалено!", "Успіх", MessageBoxButton.OK, MessageBoxImage.Information);
                        loadData();
                    }
                    else
                    {
                        MessageBox.Show("Книгу не знайдено або вже видалена.", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Помилка при видаленні: " + ex.Message, "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            


        }
    }



	public class Book {
		public required int code { get; set; }
		public required string author { get; set; }
		public required string title { get; set; }
		public required int year { get; set; }
		public required int rack { get; set; }
		public required int shelf { get; set; }
	}

	public class Authorization
	{
		public Authorization()
		{
			authorized = false;
		}
		private bool authorized;
		public bool authorize(string username, string password)
		{
			if (username == "admin" && password == "password")
			{
				authorized = true;
			}
			return authorized;
		}

		public void unauthorize() {
			authorized = false;
		}


	}

	public static class Globals
	{
		public static Authorization mainAuthorization = new Authorization();
		
	}




}
