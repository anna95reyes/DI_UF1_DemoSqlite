using DBLib.db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Text;

namespace DBLib
{
    public class DeptDB
    {
        public static int GetNumeroDepartaments()
        {
            using (SQLiteDBContext context = new SQLiteDBContext()) //crea el contexte de la base de dades
            {
                using (DbConnection connection = context.Database.GetDbConnection()) //pren la conexxio de la BD
                {
                    connection.Open();
                    using (DbCommand consulta = connection.CreateCommand())
                    {
                        consulta.CommandText = @"select count(1)
                                                from dept";

                        return (int)((long) consulta.ExecuteScalar()); //per cuan pot retorna una i nomes una fila

                    }
                }
            }
        }

        public static ObservableCollection<Dept> GetLlistaGepartament()
        {
            ObservableCollection<Dept> departaments = new ObservableCollection<Dept>();

            using(SQLiteDBContext context = new SQLiteDBContext()) //crea el contexte de la base de dades
            {
                using(DbConnection connection = context.Database.GetDbConnection()) //pren la conexxio de la BD
                {
                    connection.Open();
                    using (DbCommand consulta = connection.CreateCommand())
                    {
                        consulta.CommandText = @"select dept_no, dnom, loc
                                                from dept";

                        DbDataReader reader = consulta.ExecuteReader(); //per cuan pot retorna mes d'una fila

                        Dictionary<string, int> ordinals = new Dictionary<string, int>();
                        string[] cols = {"DEPT_NO", "DNOM", "LOC" };
                        foreach (string c in cols)
                        {
                            ordinals[c] = reader.GetOrdinal(c);
                        }


                        while (reader.Read()) //llegeix la fila seguent, retorna true si ha pogut llegir la fila, retorna false si no hi ha mes dades per lleguir
                        {
                            int dept_no = reader.GetInt32(ordinals["DEPT_NO"]);
                            string dnom = reader.GetString(ordinals["DNOM"]);
                            string loc = reader.GetString(ordinals["LOC"]);

                            Dept d = new Dept(dept_no, dnom, loc);
                            departaments.Add(d);
                        }
                    }
                }
            }
            return departaments;
        }

        public static ObservableCollection<Dept> GetLlistaGepartament(String nomDept)
        {
            ObservableCollection<Dept> departaments = new ObservableCollection<Dept>();

            using (SQLiteDBContext context = new SQLiteDBContext()) //crea el contexte de la base de dades
            {
                using (DbConnection connection = context.Database.GetDbConnection()) //pren la conexxio de la BD
                {
                    connection.Open();
                    using (DbCommand consulta = connection.CreateCommand())
                    {

                        DBUtil.crearParametre(consulta, "@param_dnom", "%" + nomDept + "%", DbType.String);

                        consulta.CommandText = $@"select dept_no, dnom, loc
                                                from dept
                                                where upper(dnom) like upper(@param_dnom)";

                        DbDataReader reader = consulta.ExecuteReader(); //per cuan pot retorna mes d'una fila

                        Dictionary<string, int> ordinals = new Dictionary<string, int>();
                        string[] cols = { "DEPT_NO", "DNOM", "LOC" };
                        foreach (string c in cols)
                        {
                            ordinals[c] = reader.GetOrdinal(c);
                        }


                        while (reader.Read()) //llegeix la fila seguent, retorna true si ha pogut llegir la fila, retorna false si no hi ha mes dades per lleguir
                        {
                            int dept_no = reader.GetInt32(ordinals["DEPT_NO"]);
                            string dnom = reader.GetString(ordinals["DNOM"]);
                            string loc = reader.GetString(ordinals["LOC"]);

                            Dept d = new Dept(dept_no, dnom, loc);
                            departaments.Add(d);
                        }
                    }
                }
            }
            return departaments;
        }

        public static void updateDepartament(Dept d)
        {
            using (SQLiteDBContext context = new SQLiteDBContext()) //crea el contexte de la base de dades
            {
                using (DbConnection connection = context.Database.GetDbConnection()) //pren la conexxio de la BD
                {
                    connection.Open();
                    DbTransaction transaccio = connection.BeginTransaction(); //Creacio d'una transaccio

                    using (DbCommand consulta = connection.CreateCommand())
                    {
                        consulta.Transaction = transaccio; // marques la consulta dins de la transacció

                        DBUtil.crearParametre(consulta, "@DNOM",    d.Dnom , DbType.String);
                        DBUtil.crearParametre(consulta, "@LOC",     d.Loc, DbType.String);
                        DBUtil.crearParametre(consulta, "@DEPT_NO", d.Dept_no, DbType.Int32);

                        consulta.CommandText = $@"update dept set DNOM=@DNOM, LOC=@LOC where DEPT_NO=@DEPT_NO";

                        int numeroDeFiles = consulta.ExecuteNonQuery(); //per fer un update o un delete
                        if(numeroDeFiles!=1)
                        {
                            //shit happens
                            transaccio.Rollback();
                        } else
                        {
                            transaccio.Commit();
                        }

                    }
                }
            }

        }



    }



}
