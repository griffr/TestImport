using operaimport_serveredition;
using Xunit.Abstractions;

namespace TestImport
{
    public class ImportTest
    {
        private int count = 200;
        private int stran_transactions = 2;
        private int sanal_lines_per_transaction = 3;
        private readonly ITestOutputHelper output;
        private string lcExternalUserName = "ckm2";
        private string lcExternalUserPassword = "ckm21234!";
        private string companyCode = "Z";
        private string importDate = DateOnly.FromDateTime(DateTime.UtcNow).ToString("dd/MM/yyyy");
        public ImportTest(ITestOutputHelper output)
        {
            this.output = output;
        }


        //Should fail but not create any transactions in Opera - one account does not exists so whole file should be rejected so when the file is fixed it can all be imported
        //Currently import will pull through any correct account codes and skip failed ones but reports back that the import was a failure where it should be a success with an exception - obviously the return type is true/false where this is a partial success
        //Either fail all or pass all - no partials
        [Fact]
        public void ImportTestStranFails()
        {
            int currentIndex = 0;
            bool result = true;
            GenerateImportFilesFailing("ST");
            string[] fileHolder = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "files"), "*.*",
                SearchOption.TopDirectoryOnly);
            foreach (var file in fileHolder)
            {
                if (Path.GetExtension(file).ToUpper() == ".CSV" &&
                    Path.GetFileNameWithoutExtension(file).ToUpper().Contains("Header".ToUpper()))
                {
                    currentIndex = currentIndex + 1;
                    output.WriteLine($"Importing file {currentIndex} of {count}");
                    clsOperaImportServerEdition clsOperaImportServerEdition = new();
                    try
                    {
                        string pairedFile = GetPairedFile(Path.Combine(Environment.CurrentDirectory, "files"), file,
                            "Header",
                            "Detail");
                        if (pairedFile == "NF")
                        {
                            Assert.True(false);
                            continue;
                        }
                        string importType = "ST";
                        string auditPass = Path.Combine(Environment.CurrentDirectory, "files",
                            $"audit{currentIndex}.txt");
                        bool vLblnSuccess = Convert.ToBoolean(clsOperaImportServerEdition.ImportData(
                            importType + "#" + lcExternalUserName + "#" + lcExternalUserPassword + "#", companyCode,
                            file + ", " + pairedFile, 3, auditPass, "U", "IMPORT",
                            DateTime.Now, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now));
                        string docReturned = Convert.ToString(clsOperaImportServerEdition.InfoCollectionItem("1"));
                        docReturned = docReturned[(docReturned.LastIndexOf('=') + 1)..];
                        output.WriteLine($"Imported {docReturned}");
                        result = vLblnSuccess;
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine(ex.Message);
                        result = false;
                    }
                    finally
                    {
                        clsOperaImportServerEdition.Clean();
                    }
                }
            }
            Assert.False(result);
        }

        //Should all pass import regardless of how many files are imported, be it 1 or 1000
        [Fact]
        public void ImportTestStran()
        {
            int currentIndex = 0;
            List<bool> results = new();
            GenerateImportFilesPassing("ST", count);
            string[] fileHolder = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "files"), "*.*",
                SearchOption.TopDirectoryOnly);
            foreach (var file in fileHolder)
            {
                if (Path.GetExtension(file).ToUpper() == ".CSV" &&
                    Path.GetFileNameWithoutExtension(file).ToUpper().Contains("Header".ToUpper()))
                {
                    currentIndex = currentIndex + 1;
                    output.WriteLine($"Importing file {currentIndex} of {count}");
                    clsOperaImportServerEdition importServerEdition = new();
                    try
                    {
                        string pairedFile = GetPairedFile(Path.Combine(Environment.CurrentDirectory, "files"), file,
                            "Header",
                            "Detail");
                        if (pairedFile == "NF")
                        {
                            Assert.True(false);
                            continue;
                        }

                        string importType = "ST";
                        string auditPass = Path.Combine(Environment.CurrentDirectory, "files",
                            $"audit-{Path.GetFileNameWithoutExtension(file)}.txt");
                        bool vLblnSuccess = Convert.ToBoolean(importServerEdition.ImportData(
                            importType + "#" + lcExternalUserName + "#" + lcExternalUserPassword + "#", companyCode,
                            file + ", " + pairedFile, 3, auditPass, "U", "IMPORT",
                            DateTime.Now, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now));
                        string docReturned = Convert.ToString(importServerEdition.InfoCollectionItem("1"));
                        docReturned = docReturned[(docReturned.LastIndexOf('=') + 1)..];
                        if (!vLblnSuccess)
                        {
                            output.WriteLine($"Failed Import {Path.GetFileName(file)} {Path.GetFileName(auditPass)}");
                        }
                        output.WriteLine($"Imported {Path.GetFileName(file)} {Path.GetFileName(auditPass)}");
                        results.Add(vLblnSuccess);
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine(ex.Message);
                        results.Add(false);
                    }
                    finally
                    {
                        importServerEdition.Clean();
                    }

                }
            }
            Assert.DoesNotContain(false, results);
        }

        //Should pass all import regardless of how many files are imported, be it 1 or 1000
        [Fact]
        public void ImportTestSop()
        {
            int currentIndex = 0;
            List<bool> results = new();
            GenerateImportFilesPassing("IT",count);
            string[] fileHolder = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "files"), "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in fileHolder)
            {
                if (Path.GetExtension(file).ToUpper() == ".CSV" &&
                    Path.GetFileNameWithoutExtension(file).ToUpper().Contains("Header".ToUpper()))
                {
                    currentIndex = currentIndex + 1;
                    output.WriteLine($"Importing file {currentIndex} of {count}");
                    clsOperaImportServerEdition clsOperaImportServerEdition = new();
                    try
                    {
                        string pairedFile = GetPairedFile(Path.Combine(Environment.CurrentDirectory, "files"), file,
                            "Header",
                            "Detail");
                        if (pairedFile == "NF")
                        {
                            Assert.True(false);
                            continue;
                        }
                        string importType = "IT";
                        string auditPass = Path.Combine(Environment.CurrentDirectory, "files",
                            $"audit{currentIndex}.txt");
                        bool vLblnSuccess = Convert.ToBoolean(clsOperaImportServerEdition.ImportData(
                            importType + "#" + lcExternalUserName + "#" + lcExternalUserPassword + "#", companyCode,
                            file + ", " + pairedFile, 3, auditPass, "U", "IMPORT",
                            DateTime.Now, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now));
                        string docReturned = Convert.ToString(clsOperaImportServerEdition.InfoCollectionItem("1"));
                        docReturned = docReturned[(docReturned.LastIndexOf('=') + 1)..];
                        output.WriteLine($"Imported {docReturned}");
                        results.Add(vLblnSuccess);
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine(ex.Message);
                        results.Add(false);
                    }
                    finally
                    {
                        clsOperaImportServerEdition.Clean();
                    }
                    
                }
            }
            
            Assert.DoesNotContain(false, results);
        }
        //Sample file imports twice in 1 import, either through dll or via opera import wizard in opera - both imports get the same document number and importer dll reports success
        [Fact]
        public void ImportTestSopSampleFiles()
        {
            int currentIndex = 0;
            List<bool> results = new();
            string[] fileHolder = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "SampleFiles"), "*.*", SearchOption.TopDirectoryOnly);
            foreach (var file in fileHolder)
            {
                if (Path.GetExtension(file).ToUpper() == ".CSV" &&
                    Path.GetFileNameWithoutExtension(file).ToUpper().Contains("head".ToUpper()))
                {
                    currentIndex = currentIndex + 1;
                    output.WriteLine($"Importing file {currentIndex} of {count}");
                    clsOperaImportServerEdition clsOperaImportServerEdition = new();
                    try
                    {
                        string pairedFile = GetPairedFile(Path.Combine(Environment.CurrentDirectory, "SampleFiles"), file,
                            "head",
                            "tran");
                        if (pairedFile == "NF")
                        {
                            Assert.True(false);
                            continue;
                        }
                        string importType = "IT";
                        string auditPass = Path.Combine(Environment.CurrentDirectory, "SampleFiles",
                            $"audit{currentIndex}.txt");
                        bool vLblnSuccess = Convert.ToBoolean(clsOperaImportServerEdition.ImportData(
                            importType + "#" + lcExternalUserName + "#" + lcExternalUserPassword + "#", companyCode,
                            file + ", " + pairedFile, 3, auditPass, "U", "IMPORT",
                            DateTime.Now, DateTime.Now.Month, DateTime.Now.Year, DateTime.Now));
                        string docReturned = Convert.ToString(clsOperaImportServerEdition.InfoCollectionItem("1"));
                        docReturned = docReturned[(docReturned.LastIndexOf('=') + 1)..];
                        output.WriteLine($"Imported {docReturned}");
                        output.WriteLine($"Check how many times it imported.");
                        results.Add(vLblnSuccess);
                    }
                    catch (Exception ex)
                    {
                        output.WriteLine(ex.Message);
                        results.Add(false);
                    }
                    finally
                    {
                        clsOperaImportServerEdition.Clean();
                    }
                    
                }
            }
            
            Assert.DoesNotContain(false, results);
        }

        private void GenerateImportFilesPassing(string type, int count)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "files")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "files"));
            string[] files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "files"));
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                output.WriteLine("All old files in the files folder deleted");
            }
            output.WriteLine($"Creating {count} pairs of files");
            string timestamp = DateTime.UtcNow.Hour.ToString() + DateTime.UtcNow.Minute.ToString() + DateTime.Now.Second.ToString();
            for (int i = 1; i <= count; i++)
            {
                if (type == "IT")
                {
                    string headerFilePath = Path.Combine(Environment.CurrentDirectory, "files", $"tempHeaderFile{i}.csv");
                    string detailFilePath = Path.Combine(Environment.CurrentDirectory, "files", $"tempDetailFile{i}.csv");
                    WriteToTextFile(headerFilePath, $"ih_account,ih_custref,ih_loc,ih_narr1,ih_narr2,ih_due,ih_doc,IH_DOCSTAT,IH_ORDDATE", false);
                    WriteToTextFile(detailFilePath, $"it_desc,it_stock,it_quan,it_anal,it_price,it_exdate,it_due,it_doc", false);
                    WriteToTextFile(headerFilePath, $"ADA0001,{type}{i}{timestamp},MAIN,{type}{i},{type}{i},{importDate},,U,{importDate}", true);
                    WriteToTextFile(detailFilePath, $"CMAT044,CMAT044,100,ACCE01,0,{importDate},{importDate}", true);
                }
                if (type == "ST")
                {
                    string headerFilePath = Path.Combine(Environment.CurrentDirectory, "files", $"tempHeaderFile{i}.csv");
                    string detailFilePath = Path.Combine(Environment.CurrentDirectory, "files", $"tempDetailFile{i}.csv");
                    decimal lineValue = 100;
                    decimal vatValue = 20;
                    decimal totalValue = (lineValue + vatValue) * sanal_lines_per_transaction;
                    decimal totalVat = vatValue * sanal_lines_per_transaction;
                    WriteToTextFile(headerFilePath, $"st_account,st_trdate,st_trref,st_trtype,st_trvalue,st_vatval,st_cbtype,st_entry", false);
                    WriteToTextFile(detailFilePath, $"SA_ACCOUNT,SA_TRDATE,SA_TRREF,SA_TRTYPE,SA_TRVALUE,sa_vatval,SA_ANCODE,SA_VATCTRY,SA_VATTYPE,SA_ANVAT", false);
                    for (int j = 1; j <= stran_transactions; j++)
                    {
                        WriteToTextFile(headerFilePath, $"ADA0001,{importDate},{type}{i}{j}{timestamp},I,{totalValue},{totalVat},,", true);
                        for(int k = 1; k <= sanal_lines_per_transaction; k++)
                        {
                            WriteToTextFile(detailFilePath, $"ADA0001,{importDate},{type}{i}{j}{timestamp},I,{lineValue},{vatValue},ACCE01,H,S,1",
                                true);
                        }
                    }
                    
                }
            }
        }
        private void GenerateImportFilesFailing(string type)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, "files")))
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "files"));
            string[] files = Directory.GetFiles(Path.Combine(Environment.CurrentDirectory, "files"));
            if (files.Length > 0)
            {
                foreach (string file in files)
                {
                    File.Delete(file);
                }
                output.WriteLine("All old files in the files folder deleted");
            }
            output.WriteLine($"Creating {1} pairs of files");

            string timestamp = DateTime.UtcNow.Hour.ToString() + DateTime.UtcNow.Minute.ToString() + DateTime.Now.Second.ToString();
            if (type == "ST")
            {
                string headerFilePath = Path.Combine(Environment.CurrentDirectory, "files", $"tempHeaderFile1.csv");
                string detailFilePath = Path.Combine(Environment.CurrentDirectory, "files", $"tempDetailFile1.csv");
                
                //Setup Headers
                WriteToTextFile(headerFilePath, $"st_account,st_trdate,st_trref,st_trtype,st_trvalue,st_vatval,st_cbtype,st_entry", false);
                WriteToTextFile(detailFilePath, $"SA_ACCOUNT,SA_TRDATE,SA_TRREF,SA_TRTYPE,SA_TRVALUE,sa_vatval,SA_ANCODE,SA_VATCTRY,SA_VATTYPE,SA_ANVAT", false);

                //Transaction 1
                WriteToTextFile(headerFilePath, $"ADA0001,{importDate},{type}F1{timestamp},I,240,40,,", true);
                WriteToTextFile(detailFilePath, $"ADA0001,{importDate},{type}F1{timestamp},I,100,20,ACCE01,H,S,1",true);
                WriteToTextFile(detailFilePath, $"ADA0001,{importDate},{type}F1{timestamp},I,100,20,ACCE01,H,S,1",true);

                //Transaction 2 should fail out the whole import not just this 1 line as account ADA0002 doesn't exist
                WriteToTextFile(headerFilePath, $"ADA0002,{importDate},{type}F2{timestamp},I,240,40,,", true);
                WriteToTextFile(detailFilePath, $"ADA0002,{importDate},{type}F2{timestamp},I,100,20,ACCE01,H,S,1",true);
                WriteToTextFile(detailFilePath, $"ADA0002,{importDate},{type}F2{timestamp},I,100,20,ACCE01,H,S,1",true);
            }
        }
        public static void WriteToTextFile(string path, string content, bool append = false)
        {
            using var fs = new FileStream(path, append ? FileMode.Append : FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            using var writer = new StreamWriter(fs);
            writer.WriteLine(content);
        }
        private static string GetPairedFile(string path, string file, string headerMask, string detailsMask)
        {
            string returnFile = Path.GetFileNameWithoutExtension(file).ToUpper();
            string[] fileHolder = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly);
            foreach (string fileList in fileHolder)
            {
                string newFile = returnFile.ToUpper().Replace(headerMask.ToUpper(), detailsMask.ToUpper()) + ".CSV";
                if (Path.GetFileName(fileList.ToUpper()) == newFile.ToUpper())
                {
                    returnFile = fileList;
                    return returnFile;
                }
            }
            return "NF";
        }
    }
}