﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Clinica_Frba.Clases;
using Clinica_Frba.Abm_de_Profesional;
using Clinica_Frba.NewFolder4;

namespace Clinica_Frba.Pedir_Turno
{
    public partial class frmTurno : Form
    {
        public frmTurno()
        {
            InitializeComponent();
        }

        public Afiliado unAfiliado = new Afiliado();
        public Profesional unProfesional = new Profesional();
        public Agenda unaAgenda = new Agenda();
        public List<Turno> listaTurnos = new List<Turno>();
        public List<Turno> listaVacia = new List<Turno>();
        public Turno unTurno = new Turno();
        public List<Turno> listaCompleta = new List<Turno>();
        public decimal unaEspecialidad { get; set; }
        public decimal nro_turno { get; set; }

        private void frmTurno_Load(object sender, EventArgs e)
        {
            try
            {
                unaAgenda.armarAgenda(unProfesional.Id, unaEspecialidad);

                if (unaAgenda.FechaDesde < DateTime.Parse(System.Configuration.ConfigurationSettings.AppSettings["Fecha"]).Date)
                {
                    dtpFechas.MinDate = DateTime.Parse(System.Configuration.ConfigurationSettings.AppSettings["Fecha"]).Date;
                }
                else
                {
                    dtpFechas.MinDate = unaAgenda.FechaDesde;
                }
                dtpFechas.MaxDate = unaAgenda.FechaHasta;

                lbl1.Text = "Afiliado: " + unAfiliado.Apellido + ", " + unAfiliado.Nombre;
            }
            catch
            {
                MessageBox.Show("El profesional seleccionado no tiene una agenda disponible", "Error", MessageBoxButtons.OK);
                lstTurno frmTurno = new lstTurno();
                frmTurno.unAfiliado = this.unAfiliado;
                frmTurno.Show();
                this.Close();
            }

            //MessageBox.Show("Desde: " + unaAgenda.FechaDesde + ", Hasta: " + unaAgenda.FechaHasta, "test", MessageBoxButtons.OK);

            grillaHorarios.AutoGenerateColumns = false;
            grillaHorarios.MultiSelect = false;

            DataGridViewTextBoxColumn ColDia = new DataGridViewTextBoxColumn();
            ColDia.DataPropertyName = "DiaString";
            ColDia.HeaderText = "Dia";
            ColDia.Width = 120;
            grillaHorarios.Columns.Add(ColDia);

            DataGridViewTextBoxColumn ColFecha = new DataGridViewTextBoxColumn();
            ColFecha.DataPropertyName = "Fecha";
            ColFecha.HeaderText = "Fecha";
            ColFecha.Width = 120;
            grillaHorarios.Columns.Add(ColFecha);

            DataGridViewTextBoxColumn ColHora = new DataGridViewTextBoxColumn();
            ColHora.DataPropertyName = "Horario";
            ColHora.HeaderText = "Horario";
            ColHora.Width = 120;
            grillaHorarios.Columns.Add(ColHora);

            lblProfesional.Text = unProfesional.Apellido + ", " + unProfesional.Nombre;
            lblEspecialidad.Text = Utiles.ObtenerEspecialidad(unaEspecialidad);
        }

        private void cmdBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Utiles.ObtenerDiasHabilesAgenda(unaAgenda).Contains(new Dias(dtpFechas.Value.DayOfWeek).Id))
                {
                    MessageBox.Show("La fecha seleccionada no forma parte de la agenda del profesional, por favor seleccione otra", "Aviso", MessageBoxButtons.OK);
                    limpiarGrilla();
                }
                else
                {
                    limpiarGrilla();

                    listaCompleta = Utiles.ObtenerTurnosAgenda(unaAgenda, ((DateTime)dtpFechas.Value).Date);

                    foreach (Turno turno in listaCompleta)
                    {
                        if(Turnos.VerificarTurnoLibre(turno)) listaTurnos.Add(turno);
                    }

                    grillaHorarios.DataSource = listaTurnos;
                }
            }
            catch
            {
                MessageBox.Show("La fecha seleccionada no esta disponible, por favor seleccione otra", "Aviso", MessageBoxButtons.OK);
                limpiarGrilla();
            }
        }

        public void limpiarGrilla()
        {
            grillaHorarios.DataSource = listaVacia;
            listaTurnos = new List<Turno>();
        }

        private void btnAction_Click(object sender, EventArgs e)
        {
            try
            {
                unTurno = (Turno)grillaHorarios.CurrentRow.DataBoundItem;
                unTurno.Codigo_Profesional = unProfesional.Id;
                unTurno.Codigo_Especialidad = unaEspecialidad;
                unTurno.Codigo_Persona = unAfiliado.Codigo_Persona;

                nro_turno = Turnos.AgregarTurno(unTurno);
                if (nro_turno != 0)
                {
                    MessageBox.Show("El turno numero " + nro_turno + " se ha registrado con exito!", "Aviso", MessageBoxButtons.OK);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("El turno que desea tomar se encuentra ocupado. Seleccione otro horario por favor.", "Error", MessageBoxButtons.OK);
                }
            }
            catch
            {
                MessageBox.Show("No se ha seleccionado ningun turno, por favor seleccione uno", "Aviso", MessageBoxButtons.OK);
            }
        }
    }
}
