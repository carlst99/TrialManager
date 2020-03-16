using Microsoft.Win32;
using System;
using System.Drawing.Printing;
using System.Printing;
using System.Printing.Interop;
using System.Runtime.InteropServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Documents.Serialization;
using System.Windows.Interop;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace TrialManager.Services
{
    /// <summary>
    /// Replacement class for System.Windows.PrintDialog
    /// </summary>
    public class ThreadSafePrintDialog
    {
        #region Fields

        private double mPrintableHeight;
        private double mPrintableWidth;

        private bool mHeightUpdated;
        private bool mWidthUpdated;

        private PrintQueue mPrintQueue;
        private PrintTicket mPrintTicket;
        private PageRange mPageRange;

        #endregion

        #region Properties

        public PageRange PageRange
        {
            get { return mPageRange; }
            set
            {
                if (value.PageTo <= 0 || value.PageFrom <= 0)
                    throw new ArgumentException("PageRange", "PageRange is not valid.");

                mPageRange = value;

                //Switch around if needed
                if (mPageRange.PageFrom > mPageRange.PageTo)
                {
                    int swap = mPageRange.PageFrom;
                    mPageRange.PageFrom = mPageRange.PageTo;
                    mPageRange.PageTo = swap;
                }
            }
        }

        public PageRangeSelection PageRangeSelection { get; set; }

        public double PrintableAreaHeight
        {
            get
            {
                if ((!mWidthUpdated && !mHeightUpdated) || (!mWidthUpdated && mHeightUpdated))
                {
                    mWidthUpdated = false;
                    mHeightUpdated = true;
                    UpdateArea();
                }

                return mPrintableHeight;
            }
        }

        public double PrintableAreaWidth
        {
            get
            {
                if ((!mWidthUpdated && !mHeightUpdated) || (mWidthUpdated && !mHeightUpdated))
                {
                    mWidthUpdated = true;
                    mHeightUpdated = false;
                    UpdateArea();
                }

                return mPrintableWidth;
            }
        }

        public PrintQueue PrintQueue
        {
            get
            {
                VerifyPrintSettings();
                return mPrintQueue;
            }
            set { mPrintQueue = value; }
        }

        public PrintTicket PrintTicket
        {
            get
            {
                VerifyPrintSettings();
                return mPrintTicket;
            }
            set { mPrintTicket = value; }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Ensure Queue and Ticket prepared
        /// </summary>
        private void VerifyPrintSettings()
        {
            if (mPrintQueue == null)
                mPrintQueue = DefaultPrintQueue();

            if (mPrintTicket == null)
                mPrintTicket = DefaultPrintTicket();
        }

        /// <summary>
        /// PrintQueue
        /// </summary>
        /// <returns></returns>
        private PrintQueue DefaultPrintQueue()
        {
            new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();
            PrintQueue queue = null;

            try
            {
                queue = new LocalPrintServer().DefaultPrintQueue;
            }
            catch (PrintSystemException)
            {
                queue = null;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }

            return queue;
        }

        /// <summary>
        /// PrintTicket
        /// </summary>
        /// <param name="printQueue"></param>
        /// <returns></returns>
        private PrintTicket DefaultPrintTicket()
        {
            new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();
            PrintTicket ticket = null;

            try
            {
                if (mPrintQueue != null)
                    ticket = mPrintQueue.UserPrintTicket ?? mPrintQueue.DefaultPrintTicket;
            }
            catch (PrintSystemException)
            {
                ticket = null;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }

            if (ticket == null)
                ticket = new PrintTicket();

            return ticket;
        }

        /// <summary>
        /// Set Print Area
        /// </summary>
        private void UpdateArea()
        {
            VerifyPrintSettings();

            PrintCapabilities caps = null;

            if (mPrintQueue != null)
                caps = mPrintQueue.GetPrintCapabilities(mPrintTicket);

            if ((caps?.OrientedPageMediaWidth.HasValue == true) && caps.OrientedPageMediaHeight.HasValue)
            {
                mPrintableWidth = caps.OrientedPageMediaWidth.Value;
                mPrintableHeight = caps.OrientedPageMediaHeight.Value;
            }
            else
            {
                mPrintableWidth = 816.0;
                mPrintableHeight = 1056.0;

                if ((mPrintTicket.PageMediaSize?.Width.HasValue == true) && mPrintTicket.PageMediaSize.Height.HasValue)
                {
                    mPrintableWidth = mPrintTicket.PageMediaSize.Width.Value;
                    mPrintableHeight = mPrintTicket.PageMediaSize.Height.Value;
                }

                if (mPrintTicket.PageOrientation.HasValue)
                {
                    if (mPrintTicket.PageOrientation.Value == PageOrientation.Landscape || mPrintTicket.PageOrientation.Value == PageOrientation.ReverseLandscape)
                    {
                        double swap = mPrintableWidth;
                        mPrintableWidth = mPrintableHeight;
                        mPrintableHeight = swap;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Actually print an Xps Document with previously setup params
        /// </summary>
        /// <param name="paginator">Document to print</param>
        /// <param name="description">Description</param>
        public void PrintDocument(DocumentPaginator paginator, string description)
        {
            if (paginator == null)
                throw new ArgumentNullException(nameof(paginator), "No DocumentPaginator to print");

            VerifyPrintSettings();

            //Handle XPS ourself, as their document writer hates our thread
            if (mPrintQueue.FullName.Contains("XPS"))
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Xps Document (*.xps) | *.xps"
                };

                if (sfd.ShowDialog() == true)
                {
                    XpsDocument document = new XpsDocument(sfd.FileName, System.IO.FileAccess.Write);
                    XpsPackagingPolicy packagePolicy = new XpsPackagingPolicy(document);
                    XpsSerializationManager serializationMgr = new XpsSerializationManager(packagePolicy, false);
                    serializationMgr.SaveAsXaml(paginator);

                    document.Close();
                }

                return;
            }

            XpsDocumentWriter writer = null;
            new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();

            try
            {
                mPrintQueue.CurrentJobSettings.Description = description;
                writer = PrintQueue.CreateXpsDocumentWriter(mPrintQueue);

                TicketEventHandler handler = new TicketEventHandler(mPrintTicket);
                writer.WritingPrintTicketRequired += handler.SetPrintTicket;
            }
            finally
            {
                CodeAccessPermission.RevertAssert();
            }

            writer.Write(paginator);

            //Reset
            mPrintableWidth = 0.0;
            mPrintableHeight = 0.0;
            mWidthUpdated = false;
            mHeightUpdated = false;
        }

        /// <summary>
        /// Show Dialog to allow user to adjust printer/settings
        /// </summary>
        /// <param name="window">Window owner</param>
        /// <returns>True is user was ready to print</returns>
        public bool ShowDialog(Window window)
        {
            NativePrintDialog dlg = new NativePrintDialog()
            {
                PrintTicket = mPrintTicket,
                PrintQueue = mPrintQueue,
                MinPage = 1,
                MaxPage = 200,
                PageRangeEnabled = false,
                PageRange = new PageRange(Math.Max(1, mPageRange.PageFrom), Math.Min(200, mPageRange.PageTo)),
                PageRangeSelection = PageRangeSelection
            };

            uint result = dlg.ShowDialog(window);
            if (result == 1 || result == 2)
            {
                mPrintQueue = dlg.PrintQueue;
                mPrintTicket = dlg.PrintTicket;
                mPageRange = dlg.PageRange;
                PageRangeSelection = dlg.PageRangeSelection;
            }

            return result == 1;
        }

        #endregion

        #region Internal

        private class TicketEventHandler
        {
            private readonly PrintTicket mPrintTicket;

            public TicketEventHandler(PrintTicket printTicket)
            {
                mPrintTicket = printTicket;
            }

            public void SetPrintTicket(object sender, WritingPrintTicketRequiredEventArgs args)
            {
                if (args.CurrentPrintTicketLevel == PrintTicketLevel.FixedDocumentSequencePrintTicket)
                    args.CurrentPrintTicket = mPrintTicket;
            }
        }

        #endregion
    }

    /// <summary>
    /// Native Printing class wrapper. Calls Window's PrintDlgEx
    /// </summary>
    internal class NativePrintDialog
    {
        #region Properties

        internal PrintQueue PrintQueue { get; set; }

        internal PrintTicket PrintTicket { get; set; }

        internal uint MinPage { get; set; } = 1;

        internal uint MaxPage { get; set; } = 0x270f;

        internal bool PageRangeEnabled { get; set; }

        internal PageRange PageRange { get; set; }

        internal PageRangeSelection PageRangeSelection { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show Dialog
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        internal uint ShowDialog(Window window)
        {
            WindowInteropHelper helper = new WindowInteropHelper(window);
            using PrintDlgEx dlg = new PrintDlgEx(helper.Handle, this);
            return dlg.ShowPrintDlgEx();
        }

        #endregion

        #region Native Class Wrapper

        private class PrintDlgEx : IDisposable
        {
            #region Fields

            private IntPtr mPrintDlgExHnd;
            private readonly NativePrintDialog mDialogOwner;
            private readonly IntPtr mWinHandle;

            #endregion

            #region Public/Internal Methods

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="owner">The Owner Handle we will attach to</param>
            /// <param name="dialog"></param>
            internal PrintDlgEx(IntPtr owner, NativePrintDialog dialog)
            {
                mWinHandle = owner;
                mDialogOwner = dialog;
                mPrintDlgExHnd = AllocatePrintDlgExStruct();
            }

            /// <summary>
            /// Show dialg and return result
            /// </summary>
            /// <returns></returns>
            internal uint ShowPrintDlgEx()
            {
                if (NativeMethods.PrintDlgEx(mPrintDlgExHnd) == 0)
                    return GetResult();
                else
                    return 0;
            }

            /// <summary>
            /// Clean up
            /// </summary>
            ~PrintDlgEx()
            {
                //Make sure object is Disposed
                Dispose(true);
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Helper method to determine CPU mode
            /// </summary>
            /// <returns></returns>
            private bool Is64Bits()
            {
                return Marshal.SizeOf(IntPtr.Zero) == 8;
            }

            /// <summary>
            /// Allocate memory associated with PRINTDLGEX32 or PRINTDLGEX64 structures
            /// </summary>
            private IntPtr AllocatePrintDlgExStruct()
            {
                NativeMethods.PRINTPAGERANGE pageRange;
                IntPtr ptr = IntPtr.Zero;
                pageRange.nToPage = (uint)mDialogOwner.PageRange.PageTo;
                pageRange.nFromPage = (uint)mDialogOwner.PageRange.PageFrom;

                try
                {
                    //Handle 32 bit case first
                    if (!Is64Bits())
                    {
                        NativeMethods.PRINTDLGEX32 pDlg = new NativeMethods.PRINTDLGEX32
                        {
                            hwndOwner = mWinHandle,
                            nMinPage = mDialogOwner.MinPage,
                            nMaxPage = mDialogOwner.MaxPage,
                            Flags = 0x9c0004
                        };

                        if (mDialogOwner.PageRangeEnabled)
                        {
                            pDlg.lpPageRanges = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.PRINTPAGERANGE)));
                            pDlg.nMaxPageRanges = 1;

                            if (mDialogOwner.PageRangeSelection == PageRangeSelection.UserPages)
                            {
                                pDlg.nPageRanges = 1;
                                Marshal.StructureToPtr(pageRange, pDlg.lpPageRanges, false);
                                pDlg.Flags |= 2;
                            }
                            else
                            {
                                pDlg.nPageRanges = 0;
                            }
                        }
                        else
                        {
                            pDlg.lpPageRanges = IntPtr.Zero;
                            pDlg.nMaxPageRanges = 0;
                            pDlg.Flags |= 8;
                        }

                        if (mDialogOwner.PrintQueue != null)
                        {
                            pDlg.hDevNames = InitializeDevNames(mDialogOwner.PrintQueue.FullName);
                            if (mDialogOwner.PrintTicket != null)
                                pDlg.hDevMode = InitializeDevMode(mDialogOwner.PrintQueue.FullName, mDialogOwner.PrintTicket);
                        }

                        ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.PRINTDLGEX32)));
                        Marshal.StructureToPtr(pDlg, ptr, false);
                        return ptr;
                    }

                    //Go with 64 bit structure
                    NativeMethods.PRINTDLGEX64 pDlg64 = new NativeMethods.PRINTDLGEX64
                    {
                        hwndOwner = mWinHandle,
                        nMinPage = mDialogOwner.MinPage,
                        nMaxPage = mDialogOwner.MaxPage,
                        Flags = 0x9c0004
                    };

                    if (mDialogOwner.PageRangeEnabled)
                    {
                        pDlg64.lpPageRanges = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.PRINTPAGERANGE)));
                        pDlg64.nMaxPageRanges = 1;

                        if (mDialogOwner.PageRangeSelection == PageRangeSelection.UserPages)
                        {
                            pDlg64.nPageRanges = 1;
                            Marshal.StructureToPtr(pageRange, pDlg64.lpPageRanges, false);
                            pDlg64.Flags |= 2;
                        }
                        else
                        {
                            pDlg64.nPageRanges = 0;
                        }
                    }
                    else
                    {
                        pDlg64.lpPageRanges = IntPtr.Zero;
                        pDlg64.nMaxPageRanges = 0;
                        pDlg64.Flags |= 8;
                    }

                    if (mDialogOwner.PrintQueue != null)
                    {
                        pDlg64.hDevNames = InitializeDevNames(mDialogOwner.PrintQueue.FullName);

                        if (mDialogOwner.PrintTicket != null)
                            pDlg64.hDevMode = InitializeDevMode(mDialogOwner.PrintQueue.FullName, mDialogOwner.PrintTicket);
                    }

                    ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(NativeMethods.PRINTDLGEX64)));
                    Marshal.StructureToPtr(pDlg64, ptr, false);
                    return ptr;
                }
                catch (Exception)
                {   //Free buffer on error
                    DeallocatePrintDlgExStruct(ptr);
                    throw;
                }
            }

            /// <summary>
            /// Frees memory associated with PRINTDLGEX32 or PRINTDLGEX64 structures
            /// </summary>
            /// <param name="ptr"></param>
            private void DeallocatePrintDlgExStruct(IntPtr ptr)
            {
                if (ptr != IntPtr.Zero)
                {
                    IntPtr hMode;
                    IntPtr hNames;
                    IntPtr hPageRanges;
                    if (Is64Bits())
                    {
                        NativeMethods.PRINTDLGEX64 pDlg = (NativeMethods.PRINTDLGEX64)Marshal.PtrToStructure(ptr, typeof(NativeMethods.PRINTDLGEX64));
                        hMode = pDlg.hDevMode;
                        hNames = pDlg.hDevNames;
                        hPageRanges = pDlg.lpPageRanges;
                    }
                    else
                    {
                        NativeMethods.PRINTDLGEX32 pDlg = (NativeMethods.PRINTDLGEX32)Marshal.PtrToStructure(ptr, typeof(NativeMethods.PRINTDLGEX32));
                        hMode = pDlg.hDevMode;
                        hNames = pDlg.hDevNames;
                        hPageRanges = pDlg.lpPageRanges;
                    }

                    if (hMode != IntPtr.Zero)
                        NativeMethods.GlobalFree(hMode);

                    if (hNames != IntPtr.Zero)
                        NativeMethods.GlobalFree(hNames);

                    if (hPageRanges != IntPtr.Zero)
                        NativeMethods.GlobalFree(hPageRanges);

                    Marshal.FreeHGlobal(ptr);
                }
            }

            /// <summary>
            /// Get the PrintQueue from the PrinterName
            /// </summary>
            /// <param name="printerName"></param>
            /// <returns></returns>
            private PrintQueue FindPrintQueue(string printerName)
            {
                EnumeratedPrintQueueTypes[] flag = new EnumeratedPrintQueueTypes[] { EnumeratedPrintQueueTypes.Local, EnumeratedPrintQueueTypes.Connections };
                PrintQueueIndexedProperty[] pArray = new PrintQueueIndexedProperty[2];
                pArray[1] = PrintQueueIndexedProperty.QueueAttributes;

                new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();

                try
                {
                    using LocalPrintServer s = new LocalPrintServer();
                    foreach (PrintQueue q in s.GetPrintQueues(pArray, flag))
                    {
                        if (printerName.Equals(q.FullName, StringComparison.OrdinalIgnoreCase))
                        {
                            q.InPartialTrust = true;
                            return q;
                        }
                    }
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }

                return null;
            }

            /// <summary>
            /// Find the PrintTicket for the queue
            /// </summary>
            /// <param name="dModeHnd"></param>
            /// <param name="printQueueName"></param>
            /// <returns></returns>
            private PrintTicket FindPrintTicket(IntPtr dModeHnd, string printQueueName)
            {
                byte[] dModeBytes = null;
                IntPtr ptr = IntPtr.Zero;

                try
                {
                    //Convert the native DevMode to a managed array of bytes
                    ptr = NativeMethods.GlobalLock(dModeHnd);
                    NativeMethods.DEVMODE dmode = (NativeMethods.DEVMODE)Marshal.PtrToStructure(ptr, typeof(NativeMethods.DEVMODE));
                    dModeBytes = new byte[dmode.dmSize + dmode.dmDriverExtra];
                    Marshal.Copy(ptr, dModeBytes, 0, dModeBytes.Length);
                }
                finally
                {
                    if (ptr != IntPtr.Zero)
                        NativeMethods.GlobalUnlock(dModeHnd);
                }

                new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();

                try
                {
                    //Convert the bytes to a native PrintTicket
                    using PrintTicketConverter converter = new PrintTicketConverter(printQueueName, PrintTicketConverter.MaxPrintSchemaVersion);
                    return converter.ConvertDevModeToPrintTicket(dModeBytes);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
            }

            /// <summary>
            /// Creates DevMode structure from Ticket
            /// </summary>
            /// <param name="printerName"></param>
            /// <param name="printTicket"></param>
            /// <returns></returns>
            private IntPtr InitializeDevMode(string printerName, PrintTicket printTicket)
            {
                new PrintingPermission(PrintingPermissionLevel.DefaultPrinting).Assert();
                byte[] dMode = null;

                try
                {
                    using PrintTicketConverter converter = new PrintTicketConverter(printerName, PrintTicketConverter.MaxPrintSchemaVersion);
                    dMode = converter.ConvertPrintTicketToDevMode(printTicket, BaseDevModeType.UserDefault);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }

                IntPtr ptrDevMode = Marshal.AllocHGlobal(dMode.Length);
                Marshal.Copy(dMode, 0, ptrDevMode, dMode.Length);
                return ptrDevMode;
            }

            /// <summary>
            /// Set DevName structure memory
            /// </summary>
            /// <param name="printerName"></param>
            /// <returns></returns>
            private IntPtr InitializeDevNames(string printerName)
            {
                IntPtr ptrDevModeNames;
                char[] strPrintName = printerName.ToCharArray();

                //Allocate native mem
                ptrDevModeNames = Marshal.AllocHGlobal(((strPrintName.Length + 3) * Marshal.SystemDefaultCharSize) + Marshal.SizeOf(typeof(NativeMethods.DEVNAMES)));

                ushort sizeOfDevNames = (ushort)Marshal.SizeOf(typeof(NativeMethods.DEVNAMES));

                //Setup structure
                NativeMethods.DEVNAMES names;
                names.wDeviceOffset = (ushort)(sizeOfDevNames / Marshal.SystemDefaultCharSize);
                names.wDriverOffset = (ushort)(names.wDeviceOffset + strPrintName.Length + 1);
                names.wOutputOffset = (ushort)(names.wDriverOffset + 1);
                names.wDefault = 0;

                //Convert to native
                Marshal.StructureToPtr(names, ptrDevModeNames, false);
                IntPtr dst = (IntPtr)(((long)ptrDevModeNames) + sizeOfDevNames);
                IntPtr dstOffset = (IntPtr)(((long)dst) + (strPrintName.Length * Marshal.SystemDefaultCharSize));

                byte[] array = new byte[3 * Marshal.SystemDefaultCharSize];
                Array.Clear(array, 0, array.Length);

                //Copy strings
                Marshal.Copy(strPrintName, 0, dst, strPrintName.Length);
                Marshal.Copy(array, 0, dstOffset, array.Length);

                return ptrDevModeNames;
            }

            /// <summary>
            /// Get dwResultAction from Dlg
            /// </summary>
            /// <param name="ptrPrintDlg"></param>
            /// <returns></returns>
            private uint GetResultPrintDlgExHnd(IntPtr ptrPrintDlg)
            {
                if (Is64Bits())
                {
                    NativeMethods.PRINTDLGEX64 dlg = (NativeMethods.PRINTDLGEX64)Marshal.PtrToStructure(ptrPrintDlg, typeof(NativeMethods.PRINTDLGEX64));
                    return dlg.dwResultAction;
                }
                else
                {
                    NativeMethods.PRINTDLGEX32 dlg = (NativeMethods.PRINTDLGEX32)Marshal.PtrToStructure(ptrPrintDlg, typeof(NativeMethods.PRINTDLGEX32));
                    return dlg.dwResultAction;
                }
            }

            /// <summary>
            /// Get various settings
            /// </summary>
            /// <param name="nativeBuffer"></param>
            /// <param name="printerName"></param>
            /// <param name="flags"></param>
            /// <param name="pageRange"></param>
            /// <param name="dModeHnd"></param>
            private void GetSettings(IntPtr nativeBuffer, out string printerName, out uint flags, out PageRange pageRange, out IntPtr dModeHnd)
            {
                IntPtr dNames;
                IntPtr pRanges;

                if (Is64Bits())
                {
                    NativeMethods.PRINTDLGEX64 dlg = (NativeMethods.PRINTDLGEX64)Marshal.PtrToStructure(nativeBuffer, typeof(NativeMethods.PRINTDLGEX64));
                    dModeHnd = dlg.hDevMode;
                    dNames = dlg.hDevNames;
                    flags = dlg.Flags;
                    pRanges = dlg.lpPageRanges;
                }
                else
                {
                    NativeMethods.PRINTDLGEX32 dlg = (NativeMethods.PRINTDLGEX32)Marshal.PtrToStructure(nativeBuffer, typeof(NativeMethods.PRINTDLGEX32));
                    dModeHnd = dlg.hDevMode;
                    dNames = dlg.hDevNames;
                    flags = dlg.Flags;
                    pRanges = dlg.lpPageRanges;
                }

                if (((flags & 2) == 2) && (pRanges != IntPtr.Zero))
                {
                    NativeMethods.PRINTPAGERANGE printRange = (NativeMethods.PRINTPAGERANGE)Marshal.PtrToStructure(pRanges, typeof(NativeMethods.PRINTPAGERANGE));
                    pageRange = new PageRange((int)printRange.nFromPage, (int)printRange.nToPage);
                }
                else
                {
                    pageRange = new PageRange(1);
                }

                if (dNames != IntPtr.Zero)
                {
                    IntPtr ptrDevNames = IntPtr.Zero;
                    try
                    {
                        ptrDevNames = NativeMethods.GlobalLock(dNames);
                        NativeMethods.DEVNAMES devnames = (NativeMethods.DEVNAMES)Marshal.PtrToStructure(ptrDevNames, typeof(NativeMethods.DEVNAMES));
                        printerName = Marshal.PtrToStringAuto((IntPtr)(((int)ptrDevNames) + (devnames.wDeviceOffset * Marshal.SystemDefaultCharSize)));
                    }
                    finally
                    {
                        if (ptrDevNames != IntPtr.Zero)
                            NativeMethods.GlobalUnlock(dNames);
                    }
                }
                else
                {
                    printerName = string.Empty;
                }
            }

            /// <summary>
            /// Get's the result from the dialog
            /// </summary>
            /// <returns></returns>
            private uint GetResult()
            {
                if (mPrintDlgExHnd == IntPtr.Zero)
                    return 0;

                uint result = GetResultPrintDlgExHnd(mPrintDlgExHnd);
                if (result == 1 || result == 2)
                {
                    GetSettings(mPrintDlgExHnd, out string printerName, out uint flags, out PageRange range, out IntPtr dModePtr);

                    mDialogOwner.PrintQueue = FindPrintQueue(printerName);
                    mDialogOwner.PrintTicket = FindPrintTicket(dModePtr, printerName);

                    if ((flags & 2) == 2)
                    {
                        if (range.PageFrom > range.PageTo)
                        {
                            int pageTo = range.PageTo;
                            range.PageTo = range.PageFrom;
                            range.PageFrom = pageTo;
                        }

                        mDialogOwner.PageRangeSelection = PageRangeSelection.UserPages;
                        mDialogOwner.PageRange = range;

                        return result;
                    }

                    mDialogOwner.PageRangeSelection = PageRangeSelection.AllPages;
                }

                return result;
            }

            #endregion

            #region IDisposable

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (disposing && mPrintDlgExHnd != IntPtr.Zero)
                {
                    DeallocatePrintDlgExStruct(mPrintDlgExHnd);
                    mPrintDlgExHnd = IntPtr.Zero;
                }
            }

            #endregion
        }

        #endregion
    }

    #region Native Method Wrappers

    /// <summary>
    /// Wrap PInvoke calls & structs
    /// </summary>
    internal static class NativeMethods
    {
        #region Native Method Wrappers

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll")]
        internal static extern IntPtr GlobalFree(IntPtr hMem);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll")]
        internal static extern IntPtr GlobalLock(IntPtr hMem);

        [SuppressUnmanagedCodeSecurity, SecurityCritical, DllImport("kernel32.dll")]
        internal static extern bool GlobalUnlock(IntPtr hMem);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
        internal static extern int PrintDlgEx(IntPtr pdex);

        #endregion

        #region Native Type Wrappers

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public int dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct DEVNAMES
        {
            public ushort wDriverOffset;
            public ushort wDeviceOffset;
            public ushort wOutputOffset;
            public ushort wDefault;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal class PRINTDLGEX32
        {
            public int lStructSize = Marshal.SizeOf(typeof(NativeMethods.PRINTDLGEX32));
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr hDevMode = IntPtr.Zero;
            public IntPtr hDevNames = IntPtr.Zero;
            public IntPtr hDC = IntPtr.Zero;
            public uint Flags;
            public uint Flags2;
            public uint ExclusionFlags;
            public uint nPageRanges;
            public uint nMaxPageRanges;
            public IntPtr lpPageRanges = IntPtr.Zero;
            public uint nMinPage;
            public uint nMaxPage;
            public uint nCopies;
            public IntPtr hInstance = IntPtr.Zero;
            public IntPtr lpPrintTemplateName = IntPtr.Zero;
            public IntPtr lpCallback = IntPtr.Zero;
            public uint nPropertyPages;
            public IntPtr lphPropertyPages = IntPtr.Zero;
            public uint nStartPage = uint.MaxValue;
            public uint dwResultAction;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 8)]
        internal class PRINTDLGEX64
        {
            public int lStructSize = Marshal.SizeOf(typeof(NativeMethods.PRINTDLGEX64));
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr hDevMode = IntPtr.Zero;
            public IntPtr hDevNames = IntPtr.Zero;
            public IntPtr hDC = IntPtr.Zero;
            public uint Flags;
            public uint Flags2;
            public uint ExclusionFlags;
            public uint nPageRanges;
            public uint nMaxPageRanges;
            public IntPtr lpPageRanges = IntPtr.Zero;
            public uint nMinPage;
            public uint nMaxPage;
            public uint nCopies;
            public IntPtr hInstance = IntPtr.Zero;
            public IntPtr lpPrintTemplateName = IntPtr.Zero;
            public IntPtr lpCallback = IntPtr.Zero;
            public uint nPropertyPages;
            public IntPtr lphPropertyPages = IntPtr.Zero;
            public uint nStartPage = uint.MaxValue;
            public uint dwResultAction;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        internal struct PRINTPAGERANGE
        {
            public uint nFromPage;
            public uint nToPage;
        }

        #endregion
    }

    #endregion
}
