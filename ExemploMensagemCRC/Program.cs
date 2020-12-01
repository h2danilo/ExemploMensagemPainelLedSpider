using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExemploMensagemCRC
{
    public class Program
    {
        static void Main(string[] args)
        {
            //instancia a classe com a implementação
            new Program();
        }

        public Program()
        {
            //classe de origem, neste caso o computador
            const byte COMPUTADOR = 0x50;
            //classe de destino, neste caso o painel de mensagens
            const byte MSG = 0xAA;


            //converte a mensagens em um array de bytes
            //byte[] message = ASCIIEncoding.ASCII.GetBytes("Danilo");

            List<byte> comando = new List<byte>();

            //byte[] message = MontaMsg1("PLACA:DWK9852", "MOEGA 1");
            byte[] message = MontaMsg2("PLACA:DWK9852", "MOEGA 1");
            //byte[] message = MontaMsg3("PLACA:DWK9852", "MOEGA 1");

            ////6X7
            //comando.Add(MSG);
            //comando.Add(0x12);

            ////TELA SUPERIOR
            //comando.Add(MSG);
            //comando.Add(0x01);

            //byte[] texto = ASCIIEncoding.ASCII.GetBytes("PLACA:DWK9852");

            //foreach (var item in texto)
            //{
            //    comando.Add(item);
            //}

            ////// Rolagem na linha inferior fixando linha superior
            //comando.Add(MSG);
            //comando.Add(0x24);

            //// Escreve na Linha Inferior (Texto Fixo)
            ////comando.Add(MSG);
            ////comando.Add(0x02);

            ////velocidade
            //comando.Add(MSG);
            //comando.Add(0x40);
            //comando.Add(0x0A);

            ////PAUSA MSG
            ////comando.Add(MSG);
            ////comando.Add(0x0F);
            ////comando.Add(0x5); //5 SEGUNDOS

            ////byte[] texto1 = ASCIIEncoding.ASCII.GetBytes("AMOEGA 1  ");
            //byte[] texto1 = ASCIIEncoding.ASCII.GetBytes("           AMOEGA 1  ");

            //foreach (var item in texto1)
            //{
            //    comando.Add(item);
            //}

            ////******** PISCANDO NO PAINEL A MSG 
            ////PAUSA MSG
            ////comando.Add(MSG);
            ////comando.Add(0x0F);
            ////comando.Add(0x2); //3 SEGUNDOS

            ////LIMPA TELA
            ////comando.Add(MSG);
            ////comando.Add(0x0B);

            ////PAUSA MSG
            ////comando.Add(MSG);
            ////comando.Add(0x0F);
            ////comando.Add(0x1); //1 SEGUNDOS
            ////****************

            //byte[] message = comando.ToArray();

            //monta o frame para atualizar a mensagem do painel
            byte[] frame = createFrameCrc(COMPUTADOR, //computador como classe de origem
                                                   0x01, //ID do grupo de origem
                                                   0x01, //ID do dispositivo de origem
                                                   MSG,  //painel de mensagens como classe de destino
                                                   0x01, //ID do grupo de destino
                                                   0x01, //ID do dispositivo de destino
                                                   0x82, //comando de atualizar o texto apresentado
                                                   0x01, //frame atual
                                                   0x01, //total de frames
                                                   message); //mensagem a ser apresentada no painel

            //objeto para conexão com o painel
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //abre a conexão passando o ip e a porta de rede do painel
            socket.Connect("192.168.15.49", 2101);

            Thread.Sleep(500);

            //envia o frame
            socket.Send(frame);

            Thread.Sleep(500);

            //fecha a conexão
            socket.Close();

            Console.Write("Pressione qualquer tecla para sair...");
            Console.ReadLine();
        }

        //TEXTO SUPERIOR FIXO E TEXTO INFERIOR ROLANDO
        public byte[] MontaMsg1 (string textoSuperior, string textoInferior)
        {
            const byte iniciafuncao = 0xAA;

            List<byte> comando = new List<byte>();

            //// Seleciona caracter 6x7
            comando.Add(iniciafuncao);
            comando.Add(0x12);

            //TELA SUPERIOR
            comando.Add(iniciafuncao);
            comando.Add(0x01);

            //MONTA TEXTO SUPERIOR
            byte[] texto = ASCIIEncoding.ASCII.GetBytes(textoSuperior);

            foreach (var item in texto)
            {
                comando.Add(item);
            }

            //Rolagem na linha inferior fixando linha superior
            comando.Add(iniciafuncao);
            comando.Add(0x24);

            //velocidade
            comando.Add(iniciafuncao);
            comando.Add(0x40);
            comando.Add(0x0A); //LENTO

            //MONTA TEXTO INFERIOR
            byte[] texto1 = ASCIIEncoding.ASCII.GetBytes("            "+ textoInferior);

            foreach (var item in texto1)
            {
                comando.Add(item);
            }

            byte[] message = comando.ToArray();

            return message;
        }

        //TEXTO SUPERIOR E INFERIOR FIXO E PISCANDO
        public byte[] MontaMsg2(string textoSuperior, string textoInferior)
        {
            const byte iniciafuncao = 0xAA;

            List<byte> comando = new List<byte>();

            //Seleciona caracter 6x7
            comando.Add(iniciafuncao);
            comando.Add(0x12);

            //TELA SUPERIOR
            comando.Add(iniciafuncao);
            comando.Add(0x01);

            //MONTA TEXTO SUPERIOR
            byte[] texto = ASCIIEncoding.ASCII.GetBytes(textoSuperior);

            foreach (var item in texto)
            {
                comando.Add(item);
            }

            //Escreve na Linha Inferior (Texto Fixo)
            comando.Add(iniciafuncao);
            comando.Add(0x02);

            //MONTA TEXTO INFERIOR
            byte[] texto1 = ASCIIEncoding.ASCII.GetBytes("            " + textoInferior);
            foreach (var item in texto1)
            {
                comando.Add(item);
            }

            //PAUSA MSG
            comando.Add(iniciafuncao);
            comando.Add(0x0F);
            comando.Add(0x2); //2 SEGUNDOS

            //LIMPA TELA
            comando.Add(iniciafuncao);
            comando.Add(0x0B);

            //PAUSA MSG
            comando.Add(iniciafuncao);
            comando.Add(0x0F);
            comando.Add(0x1); //1 SEGUNDOS

            byte[] message = comando.ToArray();

            return message;
        }

        //TEXTO SUPERIOR E INFERIOR FIXO 
        public byte[] MontaMsg3(string textoSuperior, string textoInferior)
        {
            const byte iniciafuncao = 0xAA;

            List<byte> comando = new List<byte>();

            //// Seleciona caracter 6x7
            comando.Add(iniciafuncao);
            comando.Add(0x12);

            //TELA SUPERIOR
            comando.Add(iniciafuncao);
            comando.Add(0x01);

            //MONTA TEXTO SUPERIOR
            byte[] texto = ASCIIEncoding.ASCII.GetBytes(textoSuperior);

            foreach (var item in texto)
            {
                comando.Add(item);
            }

            //Escreve na Linha Inferior (Texto Fixo)
            comando.Add(iniciafuncao);
            comando.Add(0x02);

            //MONTA TEXTO INFERIOR
            byte[] texto1 = ASCIIEncoding.ASCII.GetBytes("            " + textoInferior);
            foreach (var item in texto1)
            {
                comando.Add(item);
            }

            byte[] message = comando.ToArray();

            return message;
        }

        public byte[] createFrameCrc(   byte sourceClass,
                                        byte sourceGroup,
                                        byte sourceId,
                                        byte destClass,
                                        byte destGroup,
                                        byte destID,
                                        byte command,
                                        byte currFrame,
                                        byte totFrames,
                                        byte[] data)
        {
            //caracteres de controle (tabela ascii)
            const byte SOH = 0x01; //inicio de frame
            const byte STX = 0x02; //inicio de texto
            const byte ETX = 0x03; //fim de texto

            //lista para criar o array de bytes/frame
            List<byte> BufferFrame = new List<byte>();

            // pega o quantidade de bytes de dados que serão enviados
            ushort lenData = (ushort)data.Length;
            // inicializa a variavel onde será calculado o crc
            ushort crc = 0;

            // header do protocolo
            BufferFrame.Add(SOH);
            BufferFrame.Add(STX);

            // informações da origem do frame
            BufferFrame.Add(sourceClass);
            BufferFrame.Add(sourceGroup);
            BufferFrame.Add(sourceId);

            // informações do destino do frame
            BufferFrame.Add(destClass);
            BufferFrame.Add(destGroup);
            BufferFrame.Add(destID);

            // comando do frame
            BufferFrame.Add(command);

            // controle de sequencia e quantidade de pacotes
            BufferFrame.Add(currFrame);
            BufferFrame.Add(totFrames);

            // quantidade de dados do frame
            BufferFrame.Add((byte)(lenData >> 8));
            BufferFrame.Add((byte)lenData);

            // dados do frame
            BufferFrame.AddRange(data);

            // footer do frame
            BufferFrame.Add(ETX);

            // calculo do crc
            crc = calcCRC(BufferFrame.ToArray());

            // inclusão do calculo 
            BufferFrame.Add((byte)(crc >> 8));
            BufferFrame.Add((byte)crc);

            // retorna o frame montado
            return BufferFrame.ToArray();
        }

        public ushort calcCRC(byte[] frame)
        {
            // Buffer para valor CRC16 calculado
            ushort CRC16 = 0xFFFF;
            // Buffer para operar caracteres durante cálculo CRC16
            ushort Char_to_CRC;
            // Contador de bits do caracter a adicionar ao cálculo CRC16
            ushort Bit_Counter;
            // Flag para sinalizar se necessário cálcular com Polinômio
            ushort Polin_Flag;
            // Polinômio para cálculo do valor CRC16
            const ushort Polynom = 0x1021;
            // inicializa o contador
            ushort i = 0;
            // pega o tamanho do frame
            ushort LenFrame = (ushort)frame.Length;

            // Enquanto não processados todos os caracteres previstos
            while (LenFrame != 0)                
            {
                // Contador de bits de cada caracter a ser processado
                Bit_Counter = 8;
                // Prepara caracter do Buffer para cálculo
                Char_to_CRC = (ushort)(frame[i] * 256);
                // CRC16 = (Valor atual CRC16 EXOR'ed com caracterdo Buffer
                CRC16 = (ushort)(CRC16 ^ Char_to_CRC);

                // Enquanto não processados todos os bits do caracter atual
                while (Bit_Counter != 0)          
                {
                    // Se bit 15 do CRC atual = 1
                    if (CRC16 > 32767)             
                    {
                        // Sinaliza necessária operação com o POLINÔMIO PADRÃO
                        Polin_Flag = 1;             
                    }

                    else
                    {
                        // Sinaliza desnecessário operação com o POLINÔMIO PADRÃO
                        Polin_Flag = 0;             
                    }

                    // Shift Left valor CRC16 atual
                    CRC16 = (ushort)(CRC16 * 2);

                    // Se bit 15 do CRC atual = 1
                    if (Polin_Flag != 0)           
                    {
                        // ExOR valor CRC16 atual e polinônio padrão
                        CRC16 = (ushort)(CRC16 ^ Polynom);  
                    }

                    // Decrementa contador de bits do caracter atual
                    Bit_Counter--;                 
                }

                // Prepara ponteiro para o próximo caracter a processar
                i++;
                // Decrementa contador de caractertes a processar
                LenFrame--;                     

            }
            
            // retorna o resultado do calculo em dois bytes
            return CRC16;
        }
    }
}
